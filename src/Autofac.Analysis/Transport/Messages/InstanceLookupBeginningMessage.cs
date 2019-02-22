using System;
using Autofac.Analysis.Transport.Model;

namespace Autofac.Analysis.Transport.Messages
{
    [Serializable]
    public class InstanceLookupBeginningMessage
    {
        public InstanceLookupBeginningMessage(InstanceLookupModel instanceLookup)
        {
            InstanceLookup = instanceLookup;
        }

        public InstanceLookupModel InstanceLookup { get; }
    }
}
