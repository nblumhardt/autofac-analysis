using System;
using System.Collections.Generic;

namespace Autofac.Analysis.Engine.Application
{
    public class InstanceLookup : IApplicationItem
    {
        readonly string _id;
        readonly ResolveOperation _resolveOperation;
        readonly LifetimeScope _activationScope;
        readonly Component _component;
        readonly InstanceLookup _dependent;
        readonly IList<InstanceLookup> _dependencyLookups = new List<InstanceLookup>();
        bool _sharedInstanceReused;

        public InstanceLookup(string id, ResolveOperation resolveOperation, LifetimeScope activationScope, Component component, InstanceLookup dependent = null)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (resolveOperation == null) throw new ArgumentNullException(nameof(resolveOperation));
            if (activationScope == null) throw new ArgumentNullException(nameof(activationScope));
            if (component == null) throw new ArgumentNullException(nameof(component));
            _id = id;
            _resolveOperation = resolveOperation;
            _activationScope = activationScope;
            _component = component;
            _dependent = dependent;
        }

        public InstanceLookup Dependent
        {
            get { return _dependent; }
        }

        public ResolveOperation ResolveOperation
        {
            get { return _resolveOperation; }
        }

        public string Id
        {
            get { return _id; }
        }

        public Component Component
        {
            get { return _component; }
        }

        public LifetimeScope ActivationScope
        {
            get { return _activationScope; }
        }

        public bool SharedInstanceReused
        {
            get { return _sharedInstanceReused; }
            set { _sharedInstanceReused = value; }
        }

        public IList<InstanceLookup> DependencyLookups
        {
            get { return _dependencyLookups; }
        }
    }
}