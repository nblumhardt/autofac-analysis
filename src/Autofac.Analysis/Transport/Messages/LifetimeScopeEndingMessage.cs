using System;

namespace Autofac.Analysis.Transport.Messages
{
    [Serializable]
    public class LifetimeScopeEndingMessage
    {
        readonly string _lifetimeScopeId;

        public LifetimeScopeEndingMessage(string lifetimeScopeId)
        {
            if (lifetimeScopeId == null) throw new ArgumentNullException(nameof(lifetimeScopeId));
            _lifetimeScopeId = lifetimeScopeId;
        }

        public string LifetimeScopeId
        {
            get { return _lifetimeScopeId; }
        }
    }
}
