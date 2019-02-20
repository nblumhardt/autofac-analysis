using System;
using Autofac.Analysis.Transport.Model;

namespace Autofac.Analysis.Transport.Messages
{
    [Serializable]
    public class ResolveOperationBeginningMessage
    {
        public ResolveOperationBeginningMessage(ResolveOperationModel resolveOperation)
        {
            ResolveOperation = resolveOperation ?? throw new ArgumentNullException(nameof(resolveOperation));
        }

        public ResolveOperationModel ResolveOperation { get; }
    }
}
