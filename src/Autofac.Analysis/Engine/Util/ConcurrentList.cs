using System.Collections.Generic;
using System.Linq;

namespace Autofac.Analysis.Engine.Util
{
    class ConcurrentList<T>
    {
        readonly object _synchRoot = new object();
        readonly IList<T> _items = new List<T>();

        public void Add(T item)
        {
            lock (_synchRoot)
                _items.Add(item);
        }

        public void Remove(T item)
        {
            lock (_synchRoot)
                _items.Remove(item);
        }

        public IList<T> ToList()
        {
            lock (_synchRoot)
                return _items.ToList();
        }
    }
}
