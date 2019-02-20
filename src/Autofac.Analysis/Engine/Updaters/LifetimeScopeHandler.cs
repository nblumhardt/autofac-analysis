using System;
using Autofac.Analysis.Engine.Application;
using Autofac.Analysis.Transport.Messages;
using Autofac.Analysis.Transport.Model;

namespace Autofac.Analysis.Engine.Updaters
{
    class LifetimeScopeHandler :
        IUpdateHandler<LifetimeScopeBeginningMessage>,
        IUpdateHandler<LifetimeScopeEndingMessage>
    {
        readonly IActiveItemRepository<LifetimeScope> _lifetimeScopes;

        public LifetimeScopeHandler(IActiveItemRepository<LifetimeScope> lifetimeScopes)
        {
            _lifetimeScopes = lifetimeScopes ?? throw new ArgumentNullException(nameof(lifetimeScopes));
        }

        public void UpdateFrom(LifetimeScopeBeginningMessage message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            LifetimeScope parent = null;
            if (message.LifetimeScope.ParentLifetimeScopeId != null)
                _lifetimeScopes.TryGetItem(message.LifetimeScope.ParentLifetimeScopeId, out parent);

            var lifetimeScope = new LifetimeScope(message.LifetimeScope.Id, TagFor(message.LifetimeScope), parent);
            _lifetimeScopes.Add(lifetimeScope);

            if (parent != null)
                parent.ActiveChildren.Add(lifetimeScope);
        }

        static string TagFor(LifetimeScopeModel lifetimeScope)
        {
            return lifetimeScope.Tag == "System.Object" ? 
                null :
                lifetimeScope.Tag;
        }

        public void UpdateFrom(LifetimeScopeEndingMessage message)
        {
            if (_lifetimeScopes.TryGetItem(message.LifetimeScopeId, out var lifetimeScope))
            {
                if (lifetimeScope.Parent != null)
                    lifetimeScope.Parent.ActiveChildren.Remove(lifetimeScope);

                _lifetimeScopes.RemoveCompleted(lifetimeScope);
            }
        }
    }
}
