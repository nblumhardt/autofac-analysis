using System;
using System.Collections.Generic;
using Autofac.Analysis.Engine.Application;
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
            if (applicationEventQueue == null) throw new ArgumentNullException("applicationEventQueue");
            _applicationEventQueue = applicationEventQueue;
        }

        public void Handle(ItemCompletedEvent<InstanceLookup> applicationEvent)
        {
            if (applicationEvent.Item.SharedInstanceReused)
                return;

            var lifetime = applicationEvent.Item.ActivationScope;
            if (!lifetime.IsRootScope)
                return;

            var component = applicationEvent.Item.Component;
            if (component.IsTracked)
            {
                if (_warnedComponents.Contains(component))
                    return;

                var messageEvent = new MessageEvent(LogEventLevel.Warning, "The disposable component {ComponentId}, {ComponentDescription}, was activated in the root scope. This means the instance will be kept alive for the lifetime of the container.", component.Id, component.Description);
                _applicationEventQueue.Enqueue(messageEvent);
            }
        }
    }
}
