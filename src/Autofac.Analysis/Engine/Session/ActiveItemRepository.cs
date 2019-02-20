using System;
using System.Collections.Generic;
using Autofac.Analysis.Engine.Application;

namespace Autofac.Analysis.Engine.Session
{
    class ActiveItemRepository<TItem> : IActiveItemRepository<TItem>
        where TItem : IApplicationItem
    {
        readonly IApplicationEventQueue _applicationEventQueue;
        readonly IDictionary<string, TItem> _items = new Dictionary<string, TItem>();

        public ActiveItemRepository(IApplicationEventQueue applicationEventQueue)
        {
            if (applicationEventQueue == null) throw new ArgumentNullException(nameof(applicationEventQueue));
            _applicationEventQueue = applicationEventQueue;
        }

        public bool TryGetItem(string id, out TItem item)
        {
            return _items.TryGetValue(id, out item);
        }

        public void Add(TItem item)
        {
            _items.Add(item.Id, item);
            _applicationEventQueue.Enqueue(new ItemCreatedEvent<TItem>(item));
        }

        public void RemoveCompleted(TItem item)
        {
            _items.Remove(item.Id);
            _applicationEventQueue.Enqueue(new ItemCompletedEvent<TItem>(item));
        }
    }
}
