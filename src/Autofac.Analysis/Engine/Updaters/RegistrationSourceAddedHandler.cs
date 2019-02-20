using System;
using Autofac.Analysis.Engine.Application;
using Autofac.Analysis.Engine.Util;
using Autofac.Analysis.Transport.Messages;

namespace Autofac.Analysis.Engine.Updaters
{
    class RegistrationSourceAddedHandler : IUpdateHandler<RegistrationSourceAddedMessage>
    {
        readonly IActiveItemRepository<RegistrationSource> _registrationSources;

        public RegistrationSourceAddedHandler(IActiveItemRepository<RegistrationSource> registrationSources)
        {
            _registrationSources = registrationSources ?? throw new ArgumentNullException(nameof(registrationSources));
        }

        public void UpdateFrom(RegistrationSourceAddedMessage message)
        {
            _registrationSources.Add(new RegistrationSource(
                message.RegistrationSource.Id,
                message.RegistrationSource.Type,
                message.RegistrationSource.Description));
        }
    }
}
