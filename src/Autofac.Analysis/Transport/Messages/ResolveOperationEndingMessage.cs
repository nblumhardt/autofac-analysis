using System;

namespace Autofac.Analysis.Transport.Messages
{
    [Serializable]
    public class ResolveOperationEndingMessage
    {
        readonly string _resolveOperationId;
        readonly string _exceptionTypeAssemblyQualifiedName;
        readonly string _exceptionMessage;

        public ResolveOperationEndingMessage(string resolveOperationId, string exceptionTypeAssemblyQualifiedName = null, string exceptionMessage = null)
        {
            if (resolveOperationId == null) throw new ArgumentNullException(nameof(resolveOperationId));
            _resolveOperationId = resolveOperationId;
            _exceptionTypeAssemblyQualifiedName = exceptionTypeAssemblyQualifiedName;
            _exceptionMessage = exceptionMessage;
        }

        public string ExceptionMessage
        {
            get { return _exceptionMessage; }
        }

        public string ExceptionTypeAssemblyQualifiedName
        {
            get { return _exceptionTypeAssemblyQualifiedName; }
        }

        public string ResolveOperationId
        {
            get { return _resolveOperationId; }
        }
    }
}
