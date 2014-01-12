using System;
using System.Collections.Generic;
using System.Linq;
using Autofac.Analysis.Engine.Application;

namespace Autofac.Analysis.Engine.Analytics.LifetimeDisposalOrder
{
    class OutOfOrderDisposalDetector : IApplicationEventHandler<ItemCompletedEvent<LifetimeScope>>
    {
        readonly IApplicationEventQueue _applicationEventQueue;
        readonly ICollection<string> _descriptionsOfParentsWithWarningsIssued = new HashSet<string>();

        public OutOfOrderDisposalDetector(IApplicationEventQueue applicationEventQueue)
        {
            if (applicationEventQueue == null) throw new ArgumentNullException("applicationEventQueue");
            _applicationEventQueue = applicationEventQueue;
        }

        public void Handle(ItemCompletedEvent<LifetimeScope> applicationEvent)
        {
            var lifetimeScope = applicationEvent.Item;
            if (lifetimeScope.ActiveChildren.Count != 0)
            {
                if (_descriptionsOfParentsWithWarningsIssued.Contains(lifetimeScope.Description))
                    return;

                _descriptionsOfParentsWithWarningsIssued.Add(lifetimeScope.Description);
                var children = string.Join(", ", lifetimeScope.ActiveChildren.Select(ls => ls.Description));
                var message = string.Format("A {0} lifetime scope was disposed before its active children (including {1}).", lifetimeScope.Description, children);
                var messageEvent = new MessageEvent(MessageRelevance.Error, "Out-of-Order Disposal", message);
                _applicationEventQueue.Enqueue(messageEvent);
            }
        }
    }
}
