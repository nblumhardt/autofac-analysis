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
            if (resolveOperations == null) throw new ArgumentNullException(nameof(resolveOperations));
            if (lifetimeScopes == null) throw new ArgumentNullException(nameof(lifetimeScopes));
            if (threads == null) throw new ArgumentNullException(nameof(threads));
            _resolveOperations = resolveOperations;
            _lifetimeScopes = lifetimeScopes;
            _threads = threads;
        }

        public void UpdateFrom(ResolveOperationBeginningMessage message)
        {
            LifetimeScope lifetimeScope;
            if (!_lifetimeScopes.TryGetItem(message.ResolveOperation.LifetimeScopeId, out lifetimeScope))
                throw new InvalidOperationException("Resolve operation beginning in an unknown lifetime scope.");

            Thread thread;
            if (!_threads.TryGetItem(message.ResolveOperation.ThreadId, out thread))
            {
                thread = new Thread(message.ResolveOperation.ThreadId);
                _threads.Add(thread);
            }

            ResolveOperation parent = null;
            if (thread.ResolveOperationStack.Count != 0)
                parent = thread.ResolveOperationStack.Peek();

            MethodIdentifier callingMethod = null;
            if (message.ResolveOperation.CallingMethodName != null)
                callingMethod = new MethodIdentifier(message.ResolveOperation.CallingMethodName, TypeIdentifier.Parse(message.ResolveOperation.CallingTypeAssemblyQualifiedName));

            var resolveOperation = new ResolveOperation(message.ResolveOperation.Id, lifetimeScope, thread, parent, callingMethod);
            if (parent != null)
                parent.SubOperations.Add(resolveOperation);

            _resolveOperations.Add(resolveOperation);
            thread.ResolveOperationStack.Push(resolveOperation);
        }

        public void UpdateFrom(ResolveOperationEndingMessage message)
        {
            ResolveOperation resolveOperation;
            if (!_resolveOperations.TryGetItem(message.ResolveOperationId, out resolveOperation))
                throw new InvalidOperationException("Ending resolve operation is unknown.");

            resolveOperation.Thread.ResolveOperationStack.Pop();
            _resolveOperations.RemoveCompleted(resolveOperation);
        }
    }
}
