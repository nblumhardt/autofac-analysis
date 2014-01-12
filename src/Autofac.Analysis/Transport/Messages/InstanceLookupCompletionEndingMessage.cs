using System;

namespace Autofac.Analysis.Transport.Messages
{
    [Serializable]
    public class InstanceLookupCompletionEndingMessage
    {
        readonly string _instanceLookupId;

        public InstanceLookupCompletionEndingMessage(string instanceLookupId)
        {
            _instanceLookupId = instanceLookupId;
        }

        public string InstanceLookupId
        {
            get { return _instanceLookupId; }
        }
    }
}
