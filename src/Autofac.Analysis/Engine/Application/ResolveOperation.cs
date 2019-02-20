using System;
using System.Collections.Generic;

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
        readonly string _id;
        readonly LifetimeScope _lifetimeScope;
        readonly Thread _thread;
        readonly ResolveOperation _parent;
        readonly MethodIdentifier _callingMethod;
        readonly Stack<InstanceLookup> _instanceLookupStack = new Stack<InstanceLookup>();
        readonly IList<ResolveOperation> _subOperations = new List<ResolveOperation>();
        InstanceLookup _rootInstanceLookup;

        public ResolveOperation(string id, LifetimeScope lifetimeScope, Thread thread, ResolveOperation parent = null, MethodIdentifier callingMethod = null)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (lifetimeScope == null) throw new ArgumentNullException(nameof(lifetimeScope));
            if (thread == null) throw new ArgumentNullException(nameof(thread));
            _id = id;
            _lifetimeScope = lifetimeScope;
            _thread = thread;
            _parent = parent;
            _callingMethod = callingMethod;
        }

        public ResolveOperation Parent
        {
            get { return _parent; }
        }

        public Thread Thread
        {
            get { return _thread; }
        }

        public MethodIdentifier CallingMethod
        {
            get { return _callingMethod; }
        }

        public LifetimeScope LifetimeScope
        {
            get { return _lifetimeScope; }
        }

        public string Id
        {
            get { return _id; }
        }

        public InstanceLookup RootInstanceLookup
        {
            get { return _rootInstanceLookup; }
            set { _rootInstanceLookup = value; }
        }

        public Stack<InstanceLookup> InstanceLookupStack
        {
            get { return _instanceLookupStack; }
        }

        public IList<ResolveOperation> SubOperations
        {
            get { return _subOperations; }
        }
    }
}
