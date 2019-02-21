using System;
using Autofac.Analysis.Engine.Application;
using Autofac.Analysis.Transport.Model;
using Serilog.Events;

namespace Autofac.Analysis.Engine.Analytics.TrackedInRootScope
{
    class TrackedInstanceInRootScopeDetector :
        IApplicationEventHandler<ItemCompletedEvent<InstanceLookup>>
    {
        readonly IApplicationEventQueue _applicationEventQueue;

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
                var messageEvent = new MessageEvent(LogEventLevel.Warning,
                    "{AnalysisCode} The tracked/`IDisposable`, non-shared component {ComponentId}, {ComponentDescription}, was activated in the root scope. This often indicates a memory leak.",
                    AnalysisCodes.TrackedInRootScope,
                    component.Id, component.Description);
                _applicationEventQueue.Enqueue(messageEvent);
            }
        }
    }
}
