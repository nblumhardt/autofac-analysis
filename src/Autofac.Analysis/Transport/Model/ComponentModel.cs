using System;
using System.Collections.Generic;
using System.Linq;

namespace Autofac.Analysis.Transport.Model
{
    public class ComponentModel
    {
        public ComponentModel(
            string id,
            IEnumerable<ServiceModel> services,
            Type limitType,
            IDictionary<string, string> metadata,
            string targetComponentId,
            OwnershipModel ownership,
            SharingModel sharing,
            LifetimeModel lifetime,
            ActivatorModel activator)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (metadata == null) throw new ArgumentNullException(nameof(metadata));
            Id = id ?? throw new ArgumentNullException(nameof(id));
            LimitType = limitType ?? throw new ArgumentNullException(nameof(limitType));
            Metadata = new Dictionary<string,string>(metadata);
            Services = services.ToArray();
            TargetComponentId = targetComponentId ?? throw new ArgumentNullException(nameof(targetComponentId));
            Ownership = ownership;
            Sharing = sharing;
            Lifetime = lifetime;
            Activator = activator;
        }

        public ActivatorModel Activator { get; }

        public LifetimeModel Lifetime { get; }

        public SharingModel Sharing { get; }

        public OwnershipModel Ownership { get; }

        public Type LimitType { get; }

        public IDictionary<string, string> Metadata { get; }

        public IEnumerable<ServiceModel> Services { get; }

        public string Id { get; }

        public string TargetComponentId { get; }
    }
}
