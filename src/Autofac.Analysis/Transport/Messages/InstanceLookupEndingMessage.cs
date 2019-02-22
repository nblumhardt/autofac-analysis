using System;

namespace Autofac.Analysis.Transport.Messages
{
    [Serializable]
    public class InstanceLookupEndingMessage
    {
        public InstanceLookupEndingMessage(string instanceLookupId, bool newInstanceActivated)
        {
            InstanceLookupId = instanceLookupId;
            NewInstanceActivated = newInstanceActivated;
        }

        public string InstanceLookupId { get; }

        public bool NewInstanceActivated { get; }
    }
}
