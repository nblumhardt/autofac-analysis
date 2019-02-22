using System;

namespace Autofac.Analysis.Transport.Messages
{
    [Serializable]
    public class ResolveOperationEndingMessage
    {
        public ResolveOperationEndingMessage(string resolveOperationId, string exceptionTypeAssemblyQualifiedName = null, string exceptionMessage = null)
        {
            ResolveOperationId = resolveOperationId ?? throw new ArgumentNullException(nameof(resolveOperationId));
            ExceptionTypeAssemblyQualifiedName = exceptionTypeAssemblyQualifiedName;
            ExceptionMessage = exceptionMessage;
        }

        public string ExceptionMessage { get; }

        public string ExceptionTypeAssemblyQualifiedName { get; }

        public string ResolveOperationId { get; }
    }
}
