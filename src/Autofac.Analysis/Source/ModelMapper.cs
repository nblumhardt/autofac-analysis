using System;
using System.Diagnostics;
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
        public string GetComponentId(IComponentRegistration componentRegistration)
        {
            if (componentRegistration == null) throw new ArgumentNullException(nameof(componentRegistration));
            return componentRegistration.Id.ToString("n");
        }

        public ComponentModel GetComponentModel(IComponentRegistration componentRegistration)
        {
            return new ComponentModel(
                GetComponentId(componentRegistration),
                componentRegistration.Services.Select(GetServiceModel),
                componentRegistration.Activator.LimitType,
                componentRegistration.Metadata.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString()),
                GetComponentId(componentRegistration.Target),
                GetOwnershipModel(componentRegistration.Ownership),
                GetSharingModel(componentRegistration.Sharing),
                GetLifetimeModel(componentRegistration.Lifetime),
                GetActivatorModel(componentRegistration.Activator));
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

            var swt = service as IServiceWithType;
            var ks = service as KeyedService;

            return new ServiceModel(ks?.ServiceKey, swt?.ServiceType, service.Description);
        }

        public LifetimeScopeModel GetLifetimeScopeModel(ILifetimeScope lifetimeScope, ILifetimeScope parent = null)
        {
            string parentId = null;
            if (parent != null)
                parentId = IdTracker.GetIdOrUnknown(parent);

            return new LifetimeScopeModel(IdTracker.GetOrAssignId(lifetimeScope), lifetimeScope.Tag.ToString(), parentId);
        }

        public ResolveOperationModel GetResolveOperationModel(IResolveOperation resolveOperation, LifetimeScopeModel lifetimeScope, StackTrace callingStackTrace)
        {
            Type locationType = null;
            MethodBase locationMethod = null;
            var frames = callingStackTrace.GetFrames();
            if (frames != null)
            {
                var lastUserCode = FindResolveCall(frames);
                if (lastUserCode != null)
                {
                    locationMethod = lastUserCode;
                    locationType = lastUserCode.DeclaringType;
                }
            }

            return new ResolveOperationModel(NewId(), lifetimeScope.Id, Thread.CurrentThread.ManagedThreadId, locationType, locationMethod);
        }

        static MethodBase FindResolveCall(StackFrame[] frames)
        {
            // Nested resolve calls aren't taken into account, here.

            for (var i = frames.Length - 1; i >= 0; --i)
            {
                var f = frames[i];
                if (!IsServiceLocatorEntrypoint(f))
                    continue;

                var ri = i + 1;
                while (ri < frames.Length)
                {
                    var mdt = frames[ri].GetMethod().DeclaringType;
                    if (mdt != null)
                        break;

                    ri++;
                }

                if (ri < frames.Length)
                    return frames[ri].GetMethod();
            }

            return null;
        }

        static bool IsServiceLocatorEntrypoint(StackFrame f)
        {
            var m = f.GetMethod();
            if (m == null)
                return false;

            var dt = m.DeclaringType;
            if (dt == null)
                return false;

            // Currently we assume that any method in either of these assemblies is a
            // service location entrypoint.
            return dt.Assembly == typeof(ContainerBuilder).Assembly ||
                   dt.Assembly.FullName.StartsWith("Microsoft.Extensions.DependencyInjection") ||
                   dt.Assembly.FullName.StartsWith("Autofac.Extensions.DependencyInjection");
        }

        public InstanceLookupModel GetInstanceLookupModel(IInstanceLookup instanceLookup, ResolveOperationModel resolveOperation)
        {
            return new InstanceLookupModel(NewId(), resolveOperation.Id, GetComponentId(instanceLookup.ComponentRegistration), IdTracker.GetIdOrUnknown(instanceLookup.ActivationScope), Enumerable.Empty<ParameterModel>());
        }

        public IdTracker IdTracker { get; } = new IdTracker();

        public RegistrationSourceModel GetRegistrationSourceModel(IRegistrationSource registrationSource)
        {
            return new RegistrationSourceModel(NewId(), registrationSource.GetType(), registrationSource.ToString());
        }

        static string NewId()
        {
            return Guid.NewGuid().ToString("n");
        }
    }
}
