using System;

namespace Autofac.Analysis.Transport.Model
{
    public class RegistrationSourceModel
    {
        public RegistrationSourceModel(string id, Type type, string description)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Type = type;
            Description = description ?? throw new ArgumentNullException(nameof(description));
        }

        public Type Type { get; }

        public string Description { get; }

        public string Id { get; }
    }
}
