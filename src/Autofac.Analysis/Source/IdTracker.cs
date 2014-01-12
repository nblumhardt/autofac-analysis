using System;
using System.Collections.Concurrent;

namespace Autofac.Analysis.Source
{
    class IdTracker
    {
        public const string UnknownId = "UnknownId";

        bool _rememberIds;

        readonly ConcurrentDictionary<object, string> _instanceIds = new ConcurrentDictionary<object, string>();

        // Test only
        public void RememberIds()
        {
            _rememberIds = true;
        }

        public void ForgetId(object instance)
        {
            if (_rememberIds)
                return;

            string unused;
            if (!_instanceIds.TryRemove(instance, out unused))
                throw new InvalidOperationException("Id not present.");
        }

        public string GetOrAssignId(object instance)
        {
            return _instanceIds.GetOrAdd(instance, ls => Guid.NewGuid().ToString());
        }

        public string GetIdOrUnknown(object instance)
        {
            string value;
            if (!_instanceIds.TryGetValue(instance, out value))
                return UnknownId;
            return value;
        }

        // Test only
        public string GetIdOrFail(object instance)
        {
            string result;
            if (!_instanceIds.TryGetValue(instance, out result))
                throw new ArgumentException("Unknown instance.");
            return result;
        }
    }
}