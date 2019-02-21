using System;
using System.Linq;
using Autofac.Analysis.Engine.Analytics;
using Autofac.Analysis.Engine.Application;
using Autofac.Analysis.Engine.Session;
using Serilog;

namespace Autofac.Analysis.Display
{
    sealed class EventWriter :
        IApplicationEventHandler<ProfilerConnectedEvent>,
        IApplicationEventHandler<MessageEvent>,
        IApplicationEventHandler<ItemCreatedEvent<ResolveOperation>>,
        IApplicationEventHandler<ItemCompletedEvent<ResolveOperation>>,
        IApplicationEventHandler<ItemCreatedEvent<Component>>,
        IApplicationEventHandler<ItemCreatedEvent<RegistrationSource>>,
        IDisposable,
        IStartable
    {
        readonly IApplicationEventBus _eventBus;
        readonly ILogger _logger;

        public EventWriter(IApplicationEventBus eventBus, ILogger logger)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _logger = logger.ForContext<EventWriter>();
        }

        public void Start()
        {
            _eventBus.Subscribe(this);
        }

        public void Handle(MessageEvent applicationEvent)
        {
#pragma warning disable Serilog004 // Constant MessageTemplate verifier
            _logger.Write(applicationEvent.Level, applicationEvent.MessageTemplate, applicationEvent.Args);
#pragma warning restore Serilog004 // Constant MessageTemplate verifier
        }

        public void Dispose()
        {
            _eventBus.Unsubscribe(this);
        }

        public void Handle(ItemCreatedEvent<ResolveOperation> applicationEvent)
        {
            if (applicationEvent.Item.Parent != null)
            {
                _logger.Information("Resolve operation {ResolveOperationId} started as child of {ParentResolveOperationId}",
                        applicationEvent.Item.Id, applicationEvent.Item.Parent.Id);
            }
            else
            {
                _logger.Information("Resolve operation {ResolveOperationId} started from {CallingMethod} on {CallingType}", applicationEvent.Item.Id, applicationEvent.Item.CallingMethod, applicationEvent.Item.CallingType);
            }
        }

        public void Handle(ItemCompletedEvent<ResolveOperation> applicationEvent)
        {
            // There's a pile of data on the item here that needs to be meaninfully output (e.g. the
            // whole resolved object graph shape and what was created where).
            //
            var graph = ToObjectGraph(applicationEvent.Item.RootInstanceLookup);

            _logger
                .ForContext("Graph", graph, destructureObjects: true)
                .Information("Resolve operation {ResolveOperationId} returned an instance from {ComponentId}", applicationEvent.Item.Id, applicationEvent.Item.RootInstanceLookup.Component.Id);
        }

        public void Handle(ItemCreatedEvent<Component> applicationEvent)
        {
            _logger
                .ForContext("Component", applicationEvent.Item, destructureObjects: true)
                .Information("Component {ComponentId}, {Description}, was registered", applicationEvent.Item.Id, applicationEvent.Item.Description);
        }

        public void Handle(ItemCreatedEvent<RegistrationSource> applicationEvent)
        {
            _logger
                .ForContext("RegistrationSource", applicationEvent.Item, destructureObjects: true)
                .Information("Registration source {RegistrationSourceId}, {Description}, was added", applicationEvent.Item.Id, applicationEvent.Item.Description);
        }

        static object ToObjectGraph(InstanceLookup instanceLookup)
        {
            if (instanceLookup == null) throw new ArgumentNullException(nameof(instanceLookup));

            // Just hacking things to keep clutter down, needs cleaning up

            if (instanceLookup.SharedInstanceReused)
            {
                if (instanceLookup.DependencyLookups.Count == 0)
                {
                    return new
                    {
                        Component = instanceLookup.Component.LimitType,
                        Reused = instanceLookup.SharedInstanceReused,
                        Scope = instanceLookup.ActivationScope.Description
                    };
                }

                return new
                {
                    Component = instanceLookup.Component.LimitType,
                    Reused = instanceLookup.SharedInstanceReused,
                    Scope = instanceLookup.ActivationScope.Description,
                    Dependencies = instanceLookup.DependencyLookups.Select(ToObjectGraph).ToArray()
                };
            }

            if (instanceLookup.DependencyLookups.Count == 0)
            {
                return new
                {
                    Component = instanceLookup.Component.LimitType,
                    Scope = instanceLookup.ActivationScope.Description
                };
            }

            return new
            {
                Component = instanceLookup.Component.LimitType,
                Scope = instanceLookup.ActivationScope.Description,
                Dependencies = instanceLookup.DependencyLookups.Select(ToObjectGraph).ToArray()
            };
        }

        public void Handle(ProfilerConnectedEvent applicationEvent)
        {
            _logger.Information("Autofac analysis module connected, {Name}, {Id}", applicationEvent.Name, applicationEvent.Id);
        }
    }
}
