using System;
using System.Collections.Concurrent;

namespace Autofac.Analysis.Source
{
    class IdTracker
    {
        public const string UnknownId = "<unknown>";

        readonly ConcurrentDictionary<object, string> _instanceIds = new ConcurrentDictionary<object, string>();
        
        public void ForgetId(object instance)
        {
            if (!_instanceIds.TryRemove(instance, out var unused))
                throw new InvalidOperationException("Id not present.");
        }

        public string GetOrAssignId(object instance)
        {
            return _instanceIds.GetOrAdd(instance, ls => Guid.NewGuid().ToString("n"));
        }

        public string GetIdOrUnknown(object instance)
        {
            return _instanceIds.TryGetValue(instance, out var value) ? value : UnknownId;
        }
    }
}