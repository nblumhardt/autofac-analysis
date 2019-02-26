using System;
using System.Collections.Generic;
using System.Linq;
using Autofac.Analysis.Engine.Application;
using Autofac.Features.Metadata;
using Autofac.Features.OwnedInstances;
using Serilog.Events;

namespace Autofac.Analysis.Engine.Analytics.ServiceLocation
{
    class ServiceLocationDetector : IApplicationEventHandler<ItemCompletedEvent<ResolveOperation>>
    {
        readonly IApplicationEventQueue _applicationEventQueue;
        readonly HashSet<string> _seen = new HashSet<string>(), _warningsIssued = new HashSet<string>();

        readonly Type[] _builtInAdapterTypes =
        {
            typeof(Lazy<>),
            typeof(Lazy<,>),
            typeof(Meta<>),
            typeof(Owned<>)
        };

        public ServiceLocationDetector(IApplicationEventQueue applicationEventQueue)
        {
            _applicationEventQueue = applicationEventQueue ?? throw new ArgumentNullException(nameof(applicationEventQueue));
        }

        public void Handle(ItemCompletedEvent<ResolveOperation> applicationEvent)
        {
            if (applicationEvent == null) throw new ArgumentNullException(nameof(applicationEvent));

            var instanceLookup = applicationEvent.Item.RootInstanceLookup;

            var activationScope = instanceLookup.ActivationScope;
            if (!activationScope.IsRootScope)
                return;

            var component = instanceLookup.Component;
            if (component.Id != component.TargetComponentId &&
                component.LimitType.IsConstructedGenericType &&
                _builtInAdapterTypes.Contains(component.LimitType.GetGenericTypeDefinition()))
                return;

            if (!component.IsTracked && component.TargetComponentId != component.Id)
                return;

            if (_warningsIssued.Contains(component.Id))
                return;

            if (_seen.Contains(component.Id))
            {
                _seen.Add(component.Id);
                return;
            }

            var warning = new MessageEvent(LogEventLevel.Warning,
                "{AnalysisCode} The non-shared component {ComponentId}, {ComponentDescription}, has been resolved twice directly from the container. This usage pattern can lead to memory leaks when tracked/`IDisposable` components are present.", 
                AnalysisCodes.NonSingletonServiceLocation,
                component.Id, component.Description);

            _applicationEventQueue.Enqueue(warning);
            _warningsIssued.Add(component.Id);
        }
    }
}
