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
            _lifetimeScopes = lifetimeScopes ?? throw new ArgumentNullException(nameof(lifetimeScopes));
            _resolveOperations = resolveOperations ?? throw new ArgumentNullException(nameof(resolveOperations));
            _components = components ?? throw new ArgumentNullException(nameof(components));
            _instanceLookups = instanceLookups ?? throw new ArgumentNullException(nameof(instanceLookups));
        }

        public void UpdateFrom(InstanceLookupBeginningMessage e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));
            var instanceLookup = e.InstanceLookup;
            if (!_resolveOperations.TryGetItem(instanceLookup.ResolveOperationId, out var resolveOperation) ||
                !_lifetimeScopes.TryGetItem(instanceLookup.ActivationScopeId, out var activationScope) ||
                !_components.TryGetItem(instanceLookup.ComponentId, out var component))
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
            if (!_instanceLookups.TryGetItem(e.InstanceLookupId, out var instanceLookup))
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
            if (!_instanceLookups.TryGetItem(applicationEvent.InstanceLookupId, out var instanceLookup))
                throw new InvalidOperationException("Instance lookup completing is unknown.");

            instanceLookup.ResolveOperation.InstanceLookupStack.Push(instanceLookup);
        }

        public void UpdateFrom(InstanceLookupCompletionEndingMessage applicationEvent)
        {
            if (!_instanceLookups.TryGetItem(applicationEvent.InstanceLookupId, out var instanceLookup))
                throw new InvalidOperationException("Instance lookup completion ending is unknown.");

            var popped = instanceLookup.ResolveOperation.InstanceLookupStack.Pop();
            if (popped != instanceLookup)
                throw new InvalidOperationException("Wrong instance lookup was on the top of the stack.");
            _instanceLookups.RemoveCompleted(instanceLookup);
        }
    }
}
