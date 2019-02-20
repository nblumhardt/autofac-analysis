using System;
using System.Linq;
using Autofac.Analysis.Engine.Application;
using Autofac.Analysis.Transport.Messages;

namespace Autofac.Analysis.Engine.Updaters
{
    class ComponentAddedHandler : IUpdateHandler<ComponentAddedMessage>
    {
        readonly IActiveItemRepository<Component> _components;

        public ComponentAddedHandler(IActiveItemRepository<Component> components)
        {
            _components = components ?? throw new ArgumentNullException(nameof(components));
        }

        public void UpdateFrom(ComponentAddedMessage message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            var component = new Component(
                message.Component.Id,
                message.Component.LimitType,
                message.Component.Services.Select(svc => new Service(svc.Description, svc.ServiceType, svc.Key)),
                message.Component.Ownership,
                message.Component.Sharing,
                message.Component.Metadata,
                message.Component.Activator,
                message.Component.Lifetime,
                message.Component.TargetComponentId);
            _components.Add(component);
        }
    }
}
