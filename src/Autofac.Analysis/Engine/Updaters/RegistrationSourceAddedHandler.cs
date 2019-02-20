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
            if (registrationSources == null) throw new ArgumentNullException(nameof(registrationSources));
            _registrationSources = registrationSources;
        }

        public void UpdateFrom(RegistrationSourceAddedMessage message)
        {
            _registrationSources.Add(new RegistrationSource(
                message.RegistrationSource.Id,
                TypeIdentifier.Parse(message.RegistrationSource.TypeAssemblyQualifiedName),
                FormatDescription(message.RegistrationSource.Description)));
        }

        static string FormatDescription(string description)
        {
            string genericDescription;
            if (GenericRegistrationSourceDescriptionFormatter.TryFormat(description, out genericDescription))
                return genericDescription;

            return description;
        }
    }
}
