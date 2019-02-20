using System;

namespace Autofac.Analysis.Transport.Messages
{
    [Serializable]
    public class LifetimeScopeEndingMessage
    {
        public LifetimeScopeEndingMessage(string lifetimeScopeId)
        {
            LifetimeScopeId = lifetimeScopeId ?? throw new ArgumentNullException(nameof(lifetimeScopeId));
        }

        public string LifetimeScopeId { get; }
    }
}
