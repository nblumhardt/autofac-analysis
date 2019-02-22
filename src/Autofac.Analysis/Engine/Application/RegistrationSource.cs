using System;

namespace Autofac.Analysis.Engine.Application
{
    public class RegistrationSource : IApplicationItem
    {
        public RegistrationSource(string id, Type type, string description)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Description = description ?? throw new ArgumentNullException(nameof(description));
        }

        public Type Type { get; }

        public string Description { get; }

        public string Id { get; }
    }
}
