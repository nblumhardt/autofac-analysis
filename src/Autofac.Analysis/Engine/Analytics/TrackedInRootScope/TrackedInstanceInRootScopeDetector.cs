using System;
using System.Collections.Generic;
using Autofac.Analysis.Engine.Application;
using Autofac.Analysis.Transport.Model;
using Serilog.Events;

namespace Autofac.Analysis.Engine.Analytics.TrackedInRootScope
{
    class TrackedInstanceInRootScopeDetector :
        IApplicationEventHandler<ItemCompletedEvent<InstanceLookup>>
    {
        readonly IApplicationEventQueue _applicationEventQueue;
        readonly HashSet<Component> _warnedComponents = new HashSet<Component>();

        public TrackedInstanceInRootScopeDetector(IApplicationEventQueue applicationEventQueue)
        {
            _applicationEventQueue = applicationEventQueue ?? throw new ArgumentNullException(nameof(applicationEventQueue));
        }

        public void Handle(ItemCompletedEvent<InstanceLookup> applicationEvent)
        {
            if (applicationEvent.Item.SharedInstanceReused)
                return;

            var lifetime = applicationEvent.Item.ActivationScope;
            if (!lifetime.IsRootScope)
                return;

            var component = applicationEvent.Item.Component;
            if (component.IsTracked && component.Sharing != SharingModel.Shared)
            {
                if (_warnedComponents.Contains(component))
                    return;

                _warnedComponents.Add(component);

                var messageEvent = new MessageEvent(LogEventLevel.Warning,
                    "{AnalysisCode} The tracked/`IDisposable`, non-shared component {ComponentId}, {ComponentDescription}, was activated in the root scope. This often indicates a memory leak.",
                    AnalysisCodes.TrackedInRootScope,
                    component.Id, component.Description);
                _applicationEventQueue.Enqueue(messageEvent);
            }
        }
    }
}
