using System;
using Autofac.Analysis.Transport.Model;

namespace Autofac.Analysis.Transport.Messages
{
    [Serializable]
    public class LifetimeScopeBeginningMessage
    {
        public LifetimeScopeBeginningMessage(LifetimeScopeModel lifetimeScope)
        {
            LifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
        }

        public LifetimeScopeModel LifetimeScope { get; }
    }
}
