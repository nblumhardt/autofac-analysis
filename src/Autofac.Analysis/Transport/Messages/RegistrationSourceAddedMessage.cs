using System;
using Autofac.Analysis.Transport.Model;

namespace Autofac.Analysis.Transport.Messages
{
    [Serializable]
    public class RegistrationSourceAddedMessage
    {
        readonly RegistrationSourceModel _registrationSource;

        public RegistrationSourceAddedMessage(RegistrationSourceModel registrationSource)
        {
            if (registrationSource == null) throw new ArgumentNullException(nameof(registrationSource));
            _registrationSource = registrationSource;
        }

        public RegistrationSourceModel RegistrationSource
        {
            get { return _registrationSource; }
        }
    }
}
