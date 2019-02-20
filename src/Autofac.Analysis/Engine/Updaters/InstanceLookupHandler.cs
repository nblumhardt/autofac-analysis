using System;
using Autofac.Analysis.Engine.Application;
using Autofac.Analysis.Transport.Messages;

namespace Autofac.Analysis.Engine.Updaters
{
    public class InstanceLookupHandler :
        IUpdateHandler<InstanceLookupBeginningMessage>,
        IUpdateHandler<InstanceLookupEndingMessage>,
        IUpdateHandler<InstanceLookupCompletionBeginningMessage>,
        IUpdateHandler<InstanceLookupCompletionEndingMessage>
    {
        readonly IActiveItemRepository<LifetimeScope> _lifetimeScopes;
        readonly IActiveItemRepository<ResolveOperation> _resolveOperations;
        readonly IActiveItemRepository<Component> _components;
        readonly IActiveItemRepository<InstanceLookup> _instanceLookups;

        public InstanceLookupHandler(
            IActiveItemRepository<LifetimeScope> lifetimeScopes,
            IActiveItemRepository<ResolveOperation> resolveOperations,
            IActiveItemRepository<Component> components,
            IActiveItemRepository<InstanceLookup> instanceLookups)
        {
            if (lifetimeScopes == null) throw new ArgumentNullException(nameof(lifetimeScopes));
            if (resolveOperations == null) throw new ArgumentNullException(nameof(resolveOperations));
            if (components == null) throw new ArgumentNullException(nameof(components));
            if (instanceLookups == null) throw new ArgumentNullException(nameof(instanceLookups));
            _lifetimeScopes = lifetimeScopes;
            _resolveOperations = resolveOperations;
            _components = components;
            _instanceLookups = instanceLookups;
        }

        public void UpdateFrom(InstanceLookupBeginningMessage e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));
            var instanceLookup = e.InstanceLookup;
            ResolveOperation resolveOperation;
            LifetimeScope activationScope;
            Component component;
            if (!_resolveOperations.TryGetItem(instanceLookup.ResolveOperationId, out resolveOperation) ||
                !_lifetimeScopes.TryGetItem(instanceLookup.ActivationScopeId, out activationScope) ||
                !_components.TryGetItem(instanceLookup.ComponentId, out component))
                throw new InvalidOperationException("Instance lookup depends on an unknown item.");

            InstanceLookup item;
            if (resolveOperation.RootInstanceLookup == null)
            {
                item = new InstanceLookup(instanceLookup.Id, resolveOperation, activationScope, component);
                resolveOperation.RootInstanceLookup = item;
                resolveOperation.InstanceLookupStack.Push(item);
            }
            else
            {
                var top = resolveOperation.InstanceLookupStack.Peek();
                item = new InstanceLookup(instanceLookup.Id, resolveOperation, activationScope, component, top);
                top.DependencyLookups.Add(item);
                resolveOperation.InstanceLookupStack.Push(item);
            }
            _instanceLookups.Add(item);
        }

        public void UpdateFrom(InstanceLookupEndingMessage e)
        {
            InstanceLookup instanceLookup;
            if (!_instanceLookups.TryGetItem(e.InstanceLookupId, out instanceLookup))
                throw new InvalidOperationException("Instance lookup ending is unknown.");

            var popped = instanceLookup.ResolveOperation.InstanceLookupStack.Pop();
            if (popped != instanceLookup)
                throw new InvalidOperationException("Wrong instance lookup was on the top of the stack.");
            popped.SharedInstanceReused = !e.NewInstanceActivated;
            if (popped.SharedInstanceReused)
                _instanceLookups.RemoveCompleted(popped);
        }

        public void UpdateFrom(InstanceLookupCompletionBeginningMessage applicationEvent)
        {
            InstanceLookup instanceLookup;
            if (!_instanceLookups.TryGetItem(applicationEvent.InstanceLookupId, out instanceLookup))
                throw new InvalidOperationException("Instance lookup completing is unknown.");

            instanceLookup.ResolveOperation.InstanceLookupStack.Push(instanceLookup);
        }

        public void UpdateFrom(InstanceLookupCompletionEndingMessage applicationEvent)
        {
            InstanceLookup instanceLookup;
            if (!_instanceLookups.TryGetItem(applicationEvent.InstanceLookupId, out instanceLookup))
                throw new InvalidOperationException("Instance lookup completion ending is unknown.");

            var popped = instanceLookup.ResolveOperation.InstanceLookupStack.Pop();
            if (popped != instanceLookup)
                throw new InvalidOperationException("Wrong instance lookup was on the top of the stack.");
            _instanceLookups.RemoveCompleted(instanceLookup);
        }
    }
}
