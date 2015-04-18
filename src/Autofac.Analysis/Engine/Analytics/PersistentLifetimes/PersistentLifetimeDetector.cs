using System;
using System.Collections.Generic;
using Autofac.Analysis.Engine.Application;
using Serilog.Events;

namespace Autofac.Analysis.Engine.Analytics.PersistentLifetimes
{
    class PersistentLifetimeDetector :
        IApplicationEventHandler<ClockTickEvent>,
        IApplicationEventHandler<ItemCreatedEvent<LifetimeScope>>,
        IApplicationEventHandler<ItemCompletedEvent<LifetimeScope>>
    {
        readonly IApplicationEventQueue _applicationEventQueue;
        readonly IDictionary<LifetimeScope, DateTime> _activeScopes = new Dictionary<LifetimeScope, DateTime>();
        readonly HashSet<string> _descriptionsOfScopesWarnedAbout = new HashSet<string>();
        static readonly TimeSpan AgeThreshold = TimeSpan.FromSeconds(10);

        public PersistentLifetimeDetector(IApplicationEventQueue applicationEventQueue)
        {
            if (applicationEventQueue == null) throw new ArgumentNullException("applicationEventQueue");
            _applicationEventQueue = applicationEventQueue;
        }

        public void Handle(ClockTickEvent applicationEvent)
        {
            var keysToRemove = new List<LifetimeScope>();

            var earliestAllowableCreationTime = DateTime.Now - AgeThreshold;
            foreach (var activeScope in _activeScopes)
            {
                var lifetimeScope = activeScope.Key;
                var createdAt = activeScope.Value;

                if (createdAt < earliestAllowableCreationTime)
                {
                    if (!_descriptionsOfScopesWarnedAbout.Contains(lifetimeScope.Description))
                    {
                        _descriptionsOfScopesWarnedAbout.Add(lifetimeScope.Description);
                        var messageEvent = new MessageEvent(LogEventLevel.Warning, 
                            "A {LifetimeScopeDescription} lifetime scope, {LifetimeScopeId}, has been active for more than {AgeThresholdSeconds} seconds. To ensure that components are properly released, lifetime scopes must be disposed when no longer required.", lifetimeScope.Description, lifetimeScope.Id, AgeThreshold.TotalSeconds);
                        _applicationEventQueue.Enqueue(messageEvent);
                    }

                    keysToRemove.Add(lifetimeScope);
                }
            }

            foreach (var removed in keysToRemove)
                _activeScopes.Remove(removed);
        }

        public void Handle(ItemCreatedEvent<LifetimeScope> applicationEvent)
        {
            if (applicationEvent.Item.IsRootScope)
                return;

            _activeScopes.Add(applicationEvent.Item, DateTime.Now);
        }

        public void Handle(ItemCompletedEvent<LifetimeScope> applicationEvent)
        {
            if (applicationEvent.Item.IsRootScope)
                return;

            if (_activeScopes.ContainsKey(applicationEvent.Item))
                _activeScopes.Remove(applicationEvent.Item);
        }
    }
}
