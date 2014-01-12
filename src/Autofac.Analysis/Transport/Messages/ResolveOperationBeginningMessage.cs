using System;
using Autofac.Analysis.Transport.Model;

namespace Autofac.Analysis.Transport.Messages
{
    [Serializable]
    public class ResolveOperationBeginningMessage
    {
        readonly ResolveOperationModel _resolveOperation;

        public ResolveOperationBeginningMessage(ResolveOperationModel resolveOperation)
        {
            if (resolveOperation == null) throw new ArgumentNullException("resolveOperation");
            _resolveOperation = resolveOperation;
        }

        public ResolveOperationModel ResolveOperation
        {
            get { return _resolveOperation; }
        }
    }
}
