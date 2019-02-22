using System;

namespace Autofac.Analysis.Transport.Messages
{
    [Serializable]
    public class InstanceLookupCompletionBeginningMessage
    {
        public InstanceLookupCompletionBeginningMessage(string instanceLookupId)
        {
            InstanceLookupId = instanceLookupId;
        }

        public string InstanceLookupId { get; }
    }
}
