using System;
using System.Collections.Generic;
using Autofac.Analysis.Engine.Application;

namespace Autofac.Analysis.Engine.Analytics.Leaks
{
    class RootScopeLeakDetector : IApplicationEventHandler<ItemCompletedEvent<ResolveOperation>>
    {
        const int ResolveOperationsBeforeWarning = 2;
        const int MaxActivationScopesToTrackPerComponent = 1;
        const string OwnedTypeFullName = "Autofac.Features.OwnedInstances.Owned";

        readonly IApplicationEventQueue _applicationEventQueue;
        readonly IDictionary<string, LastNActivationsScopeTracker> _componentToScopeTrackers = new Dictionary<string, LastNActivationsScopeTracker>();

        public RootScopeLeakDetector(IApplicationEventQueue applicationEventQueue)
        {
            if (applicationEventQueue == null) throw new ArgumentNullException("applicationEventQueue");
            _applicationEventQueue = applicationEventQueue;
        }

        public void Handle(ItemCompletedEvent<ResolveOperation> applicationEvent)
        {
            if (applicationEvent == null) throw new ArgumentNullException("applicationEvent");

            var instanceLookup = applicationEvent.Item.RootInstanceLookup;
            if (instanceLookup.SharedInstanceReused)
                return;

            var activationScope = instanceLookup.ActivationScope;
            if (!activationScope.IsRootScope)
                return;

            var component = instanceLookup.Component;
            if (component.LimitType.Identity.FullName == OwnedTypeFullName)
                return;

            LastNActivationsScopeTracker tracker;
            if (!_componentToScopeTrackers.TryGetValue(component.Id, out tracker))
            {
                tracker = new LastNActivationsScopeTracker(MaxActivationScopesToTrackPerComponent);
                _componentToScopeTrackers[component.Id] = tracker;
            }

            if (tracker.HasWarningBeenIssued)
                return;

            var resolveOperationsInThisScope = tracker.RecordActivation(activationScope.Id);

            if (resolveOperationsInThisScope == ResolveOperationsBeforeWarning)
            {
                var message = string.Format("The component {0} has been resolved twice directly from the container. This can indicate a potential memory leak.", component.Description);
                var warning = new MessageEvent(MessageRelevance.Warning, "Possible Memory Leak", message);
                _applicationEventQueue.Enqueue(warning);
                tracker.HasWarningBeenIssued = true;
            }
        }
    }
}
