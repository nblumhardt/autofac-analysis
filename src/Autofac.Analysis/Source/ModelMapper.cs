using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using Autofac.Analysis.Transport.Model;
using Autofac.Core;
using Autofac.Core.Activators.Delegate;
using Autofac.Core.Activators.ProvidedInstance;
using Autofac.Core.Activators.Reflection;
using Autofac.Core.Lifetime;
using Autofac.Core.Resolving;

namespace Autofac.Analysis.Source
{
    class ModelMapper
    {
        readonly IdTracker _idTracker = new IdTracker();
        readonly ConcurrentDictionary<Type, TypeModel> _typeModels = new ConcurrentDictionary<Type, TypeModel>();

        public string GetComponentId(IComponentRegistration componentRegistration)
        {
            if (componentRegistration == null) throw new ArgumentNullException(nameof(componentRegistration));
            return componentRegistration.Id.ToString();
        }

        public ComponentModel GetComponentModel(IComponentRegistration componentRegistration)
        {
            return new ComponentModel(
                GetComponentId(componentRegistration),
                componentRegistration.Services.Select(GetServiceModel),
                GetTypeId(componentRegistration.Activator.LimitType),
                componentRegistration.Metadata.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString()),
                GetComponentId(componentRegistration.Target),
                GetOwnershipModel(componentRegistration.Ownership),
                GetSharingModel(componentRegistration.Sharing),
                GetLifetimeModel(componentRegistration.Lifetime),
                GetActivatorModel(componentRegistration.Activator));
        }

        public bool GetOrAddTypeModel(Type type, out TypeModel typeModel)
        {
            TypeModel created = null;
            typeModel = _typeModels.GetOrAdd(type, t => created = MapType(type));
            return created == typeModel;
        }

        static TypeModel MapType(Type type)
        {
            return new TypeModel(
                NewId(),
                type.AssemblyQualifiedName,
                typeof(IDisposable).IsAssignableFrom(type));
        }

        public ActivatorModel GetActivatorModel(IInstanceActivator activator)
        {
            if (activator == null) throw new ArgumentNullException(nameof(activator));

            if (activator is ReflectionActivator)
                return ActivatorModel.Reflection;

            if (activator is DelegateActivator)
                return ActivatorModel.Delegate;

            if (activator is ProvidedInstanceActivator)
                return ActivatorModel.ProvidedInstance;

            return ActivatorModel.Other;
        }

        public LifetimeModel GetLifetimeModel(IComponentLifetime lifetime)
        {
            if (lifetime == null) throw new ArgumentNullException(nameof(lifetime));

            if (lifetime is CurrentScopeLifetime)
                return LifetimeModel.CurrentScope;

            if (lifetime is RootScopeLifetime)
                return LifetimeModel.RootScope;

            if (lifetime is MatchingScopeLifetime)
                return LifetimeModel.MatchingScope;

            return LifetimeModel.Other;
        }

        public SharingModel GetSharingModel(InstanceSharing sharing)
        {
            return (SharingModel) (int) sharing;
        }

        public OwnershipModel GetOwnershipModel(InstanceOwnership ownership)
        {
            return (OwnershipModel) (int) ownership;
        }

        public ServiceModel GetServiceModel(Service service)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));

            string key = null, serviceTypeId = null;

            var swt = service as IServiceWithType;
            if (swt != null)
                serviceTypeId = GetTypeId(swt.ServiceType);

            var ks = service as KeyedService;
            if (ks != null)
                key = ks.ServiceKey.ToString();

            return new ServiceModel(key, serviceTypeId, service.Description);
        }

        public LifetimeScopeModel GetLifetimeScopeModel(ILifetimeScope lifetimeScope, ILifetimeScope parent = null)
        {
            string parentId = null;
            if (parent != null)
                parentId = _idTracker.GetIdOrUnknown(parent);

            return new LifetimeScopeModel(_idTracker.GetOrAssignId(lifetimeScope), lifetimeScope.Tag.ToString(), parentId);
        }

        public ResolveOperationModel GetResolveOperationModel(IResolveOperation resolveOperation, LifetimeScopeModel lifetimeScope, StackTrace callingStackTrace)
        {
            string locationTypeAssemblyQualifiedName = null, locationMethodName = null;
            var frames = callingStackTrace.GetFrames();
            if (frames != null)
            {
                var lastUserCode = FindResolveCall(frames);
                if (lastUserCode != null)
                {
                    locationMethodName = lastUserCode.Name;
                    locationTypeAssemblyQualifiedName = lastUserCode.DeclaringType.AssemblyQualifiedName;
                }
            }

            return new ResolveOperationModel(NewId(), lifetimeScope.Id, GetThreadId(Thread.CurrentThread), locationTypeAssemblyQualifiedName, locationMethodName);
        }

        static MethodBase FindResolveCall(StackFrame[] frames)
        {
            for (var i = frames.Length - 1; i >= 0; --i)
            {
                var f = frames[i];
                var m = f.GetMethod();
                if (m != null)
                {
                    var dt = m.DeclaringType;
                    if (dt != null)
                    {
                        if (i < frames.Length - 1 && dt.Assembly == typeof(ContainerBuilder).Assembly)
                        {
                            var ri = i + 1;
                            while (ri < frames.Length && frames[ri].GetMethod().DeclaringType == null)
                                ri++;

                            if (ri < frames.Length)
                                return frames[ri].GetMethod();
                        }
                    }
                }
            }

            return null;
        }

        public InstanceLookupModel GetInstanceLookupModel(IInstanceLookup instanceLookup, ResolveOperationModel resolveOperation)
        {
            return new InstanceLookupModel(NewId(), resolveOperation.Id, GetComponentId(instanceLookup.ComponentRegistration), _idTracker.GetIdOrUnknown(instanceLookup.ActivationScope), Enumerable.Empty<ParameterModel>());
        }

        public IdTracker IdTracker { get { return _idTracker; } }

        public RegistrationSourceModel GetRegistrationSourceModel(IRegistrationSource registrationSource)
        {
            return new RegistrationSourceModel(NewId(), registrationSource.GetType().AssemblyQualifiedName, registrationSource.ToString());
        }

        static string NewId()
        {
            return Guid.NewGuid().ToString();
        }

        // Internal ids are uniformly strings
        public string GetThreadId(Thread thread)
        {
            return thread.ManagedThreadId.ToString(CultureInfo.InvariantCulture);
        }

        public string GetTypeId(Type type)
        {
            TypeModel tm;
            if (GetOrAddTypeModel(type, out tm))
                throw new InvalidOperationException(string.Format("No model has been created for type '{0}'", type.AssemblyQualifiedName));
            return tm.Id;
        }

        public IEnumerable<Type> GetReferencedTypes(IComponentRegistration registration)
        {
            yield return registration.Activator.LimitType;
            foreach (var st in registration.Services.OfType<IServiceWithType>())
                yield return st.ServiceType;
        }
    }
}
