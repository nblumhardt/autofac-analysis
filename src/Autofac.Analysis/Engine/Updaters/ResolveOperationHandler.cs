using System;
using Autofac.Analysis.Engine.Application;
using Autofac.Analysis.Transport.Messages;

namespace Autofac.Analysis.Engine.Updaters
{
    public class ResolveOperationHandler : 
        IUpdateHandler<ResolveOperationBeginningMessage>,
        IUpdateHandler<ResolveOperationEndingMessage>
    {
        readonly IActiveItemRepository<ResolveOperation> _resolveOperations;
        readonly IActiveItemRepository<LifetimeScope> _lifetimeScopes;
        readonly IActiveItemRepository<Thread> _threads;

        public ResolveOperationHandler(
            IActiveItemRepository<ResolveOperation> resolveOperations,
            IActiveItemRepository<LifetimeScope> lifetimeScopes,
            IActiveItemRepository<Thread> threads)
        {
            _resolveOperations = resolveOperations ?? throw new ArgumentNullException(nameof(resolveOperations));
            _lifetimeScopes = lifetimeScopes ?? throw new ArgumentNullException(nameof(lifetimeScopes));
            _threads = threads ?? throw new ArgumentNullException(nameof(threads));
        }

        public void UpdateFrom(ResolveOperationBeginningMessage message)
        {
            if (!_lifetimeScopes.TryGetItem(message.ResolveOperation.LifetimeScopeId, out var lifetimeScope))
                throw new InvalidOperationException("Resolve operation beginning in an unknown lifetime scope.");

            if (!_threads.TryGetItem(message.ResolveOperation.ManagedThreadId.ToString(), out var thread))
            {
                thread = new Thread(message.ResolveOperation.ManagedThreadId.ToString());
                _threads.Add(thread);
            }

            ResolveOperation parent = null;
            if (thread.ResolveOperationStack.Count != 0)
                parent = thread.ResolveOperationStack.Peek();
            
            var resolveOperation = new ResolveOperation(message.ResolveOperation.Id, lifetimeScope, thread, parent, message.ResolveOperation.CallingType, message.ResolveOperation.CallingMethod);
            if (parent != null)
                parent.SubOperations.Add(resolveOperation);

            _resolveOperations.Add(resolveOperation);
            thread.ResolveOperationStack.Push(resolveOperation);
        }

        public void UpdateFrom(ResolveOperationEndingMessage message)
        {
            if (!_resolveOperations.TryGetItem(message.ResolveOperationId, out var resolveOperation))
                throw new InvalidOperationException("Ending resolve operation is unknown.");

            resolveOperation.Thread.ResolveOperationStack.Pop();
            _resolveOperations.RemoveCompleted(resolveOperation);
        }
    }
}
