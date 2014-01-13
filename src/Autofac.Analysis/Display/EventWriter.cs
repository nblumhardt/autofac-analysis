using System;
using Autofac.Analysis.Engine.Analytics;
using Autofac.Analysis.Engine.Application;
using Autofac.Analysis.Engine.Session;
using Serilog;
using Serilog.Events;

namespace Autofac.Analysis.Display
{
    sealed class EventWriter :
        IApplicationEventHandler<MessageEvent>,
        IApplicationEventHandler<ItemCreatedEvent<ResolveOperation>>,
        IApplicationEventHandler<ItemCompletedEvent<ResolveOperation>>,
        IDisposable,
        IStartable
    {
        readonly IApplicationEventBus _eventBus;
        readonly ILogger _logger;

        public EventWriter(IApplicationEventBus eventBus)
        {
            if (eventBus == null) throw new ArgumentNullException("eventBus");
            _eventBus = eventBus;
            _logger = Log.ForContext<EventWriter>();
        }

        public void Start()
        {
            _eventBus.Subscribe(this);
        }

        public void Handle(MessageEvent applicationEvent)
        {
            var level = LogEventLevel.Information;
            if (applicationEvent.Relevance == MessageRelevance.Error)
                level = LogEventLevel.Error;
            else if (applicationEvent.Relevance == MessageRelevance.Warning)
                level = LogEventLevel.Warning;

            _logger.Write(level, "{Title}: {Message}", applicationEvent.Title, applicationEvent.Message);
        }

        public void Dispose()
        {
            _eventBus.Unsubscribe(this);
        }

        public void Handle(ItemCreatedEvent<ResolveOperation> applicationEvent)
        {
            _logger.Information("Resolve operation {ResolveOperationId} started", applicationEvent.Item.Id);
        }

        public void Handle(ItemCompletedEvent<ResolveOperation> applicationEvent)
        {
            // There's a pile of data on the item here that needs to be meaninfully output (e.g. the
            // whole resolved object graph shape and what was created where).
            _logger.Information("Resolve operation {ResolveOperationId} completed", applicationEvent.Item.Id);
        }
    }
}
