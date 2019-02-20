using System;
using System.Collections.Generic;
using System.Reflection;

namespace Autofac.Analysis.Engine.Application
{
    /// <summary>
    /// A <see cref="ResolveOperation" /> is a cascade of instance lookups generated when Resolve() is called
    /// on a component context. Typically, only a single resolve operation will occur per object graph, but, it's
    /// possible to recursively call Resolve() against the outer container during such an operation, so effectively
    /// resolve operations also form a tree. This will also be apparent when resolve operations cross to a child
    /// scope as occurs with Owned.
    /// </summary>
    public class ResolveOperation : IApplicationItem
    {
        public ResolveOperation(string id, LifetimeScope lifetimeScope, Thread thread, ResolveOperation parent = null, MethodBase callingMethod = null)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            LifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
            Thread = thread ?? throw new ArgumentNullException(nameof(thread));
            Parent = parent;
            CallingMethod = callingMethod;
        }

        public ResolveOperation Parent { get; }

        public Thread Thread { get; }

        public MethodBase CallingMethod { get; }

        public LifetimeScope LifetimeScope { get; }

        public string Id { get; }

        public InstanceLookup RootInstanceLookup { get; set; }

        public Stack<InstanceLookup> InstanceLookupStack { get; } = new Stack<InstanceLookup>();

        public IList<ResolveOperation> SubOperations { get; } = new List<ResolveOperation>();
    }
}
