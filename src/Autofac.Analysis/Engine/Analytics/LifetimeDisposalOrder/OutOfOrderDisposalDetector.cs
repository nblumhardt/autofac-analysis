using System;
using System.Collections.Generic;
using System.Linq;
using Autofac.Analysis.Engine.Application;
using Serilog.Events;

namespace Autofac.Analysis.Engine.Analytics.LifetimeDisposalOrder
{
    class OutOfOrderDisposalDetector : IApplicationEventHandler<ItemCompletedEvent<LifetimeScope>>
    {
        readonly IApplicationEventQueue _applicationEventQueue;
        readonly ICollection<string> _descriptionsOfParentsWithWarningsIssued = new HashSet<string>();

        public OutOfOrderDisposalDetector(IApplicationEventQueue applicationEventQueue)
        {
            _applicationEventQueue = applicationEventQueue ?? throw new ArgumentNullException(nameof(applicationEventQueue));
        }

        public void Handle(ItemCompletedEvent<LifetimeScope> applicationEvent)
        {
            var lifetimeScope = applicationEvent.Item;
            if (lifetimeScope.ActiveChildren.Count != 0)
            {
                if (_descriptionsOfParentsWithWarningsIssued.Contains(lifetimeScope.Description))
                    return;

                _descriptionsOfParentsWithWarningsIssued.Add(lifetimeScope.Description);
                var childScopeDescriptions = lifetimeScope.ActiveChildren.Select(ls => ls.Description).ToArray();
                var messageEvent = new MessageEvent(LogEventLevel.Error, 
                    "{AnalysisCode} A {LifetimeScopeDescription} lifetime scope, {LifetimeScopeId}, was disposed before its active children (including {ChildScopeDescriptions}).",
                    AnalysisCodes.LifetimeScopeDisposalOrdering,
                    lifetimeScope.Description,
                    lifetimeScope.Id,
                    childScopeDescriptions);
                _applicationEventQueue.Enqueue(messageEvent);
            }
        }
    }
}
