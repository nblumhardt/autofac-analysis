using System;

namespace Autofac.Analysis.Transport.Messages
{
    [Serializable]
    public class InstanceLookupCompletionEndingMessage
    {
        public InstanceLookupCompletionEndingMessage(string instanceLookupId)
        {
            InstanceLookupId = instanceLookupId;
        }

        public string InstanceLookupId { get; }
    }
}
