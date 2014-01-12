using System;

namespace Autofac.Analysis.Transport.Messages
{
    [Serializable]
    public class InstanceLookupEndingMessage
    {
        readonly string _instanceLookupId;
        readonly bool _newInstanceActivated;

        public InstanceLookupEndingMessage(string instanceLookupId, bool newInstanceActivated)
        {
            _instanceLookupId = instanceLookupId;
            _newInstanceActivated = newInstanceActivated;
        }

        public string InstanceLookupId
        {
            get { return _instanceLookupId; }
        }

        public bool NewInstanceActivated
        {
            get { return _newInstanceActivated; }
        }
    }
}
