using System;
using System.Collections.Generic;
using System.Linq;
using Autofac.Analysis.Transport.Model;
using Autofac.Analysis.Util;

namespace Autofac.Analysis.Engine.Application
{
    public class Component : IApplicationItem
    {
        public Component(string id, Type limitType, IEnumerable<Service> services, OwnershipModel ownership, SharingModel sharing, IDictionary<string, string> metadata, ActivatorModel activator, LifetimeModel lifetime, string targetComponentId = null)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            Id = id;
            LimitType = limitType ?? throw new ArgumentNullException(nameof(limitType));
            Services = services.ToArray();
            Ownership = ownership;
            Sharing = sharing;
            Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
            Activator = activator;
            Lifetime = lifetime;
            TargetComponentId = targetComponentId;
        }

        public string TargetComponentId { get; }

        public LifetimeModel Lifetime { get; }

        public ActivatorModel Activator { get; }

        public SharingModel Sharing { get; }

        public OwnershipModel Ownership { get; }

        public IEnumerable<Service> Services { get; }

        public Type LimitType { get; }

        public string Id { get; }

        public string Description
        {
            get
            {
                if (LimitType != typeof(object))
                    return LimitType.ToString();

                return "Unknown (" + string.Join(", ", Services.Select(s => s.ServiceType)) + ")";
            }
        }

        public bool IsTracked => LimitType.IsDisposable() && Ownership == OwnershipModel.OwnedByLifetimeScope;

        public IDictionary<string, string> Metadata { get; }
    }
}
