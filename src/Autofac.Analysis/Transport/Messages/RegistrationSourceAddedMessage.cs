using System;
using Autofac.Analysis.Transport.Model;

namespace Autofac.Analysis.Transport.Messages
{
    [Serializable]
    public class RegistrationSourceAddedMessage
    {
        public RegistrationSourceAddedMessage(RegistrationSourceModel registrationSource)
        {
            RegistrationSource = registrationSource ?? throw new ArgumentNullException(nameof(registrationSource));
        }

        public RegistrationSourceModel RegistrationSource { get; }
    }
}
