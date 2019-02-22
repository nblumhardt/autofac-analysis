using System;
using System.Collections.Generic;

namespace Autofac.Analysis.Engine.Application
{
    public class InstanceLookup : IApplicationItem
    {
        public InstanceLookup(string id, ResolveOperation resolveOperation, LifetimeScope activationScope, Component component, InstanceLookup dependent = null)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            ResolveOperation = resolveOperation ?? throw new ArgumentNullException(nameof(resolveOperation));
            ActivationScope = activationScope ?? throw new ArgumentNullException(nameof(activationScope));
            Component = component ?? throw new ArgumentNullException(nameof(component));
            Dependent = dependent;
        }

        public InstanceLookup Dependent { get; }

        public ResolveOperation ResolveOperation { get; }

        public string Id { get; }

        public Component Component { get; }

        public LifetimeScope ActivationScope { get; }

        public bool SharedInstanceReused { get; set; }

        public IList<InstanceLookup> DependencyLookups { get; } = new List<InstanceLookup>();
    }
}