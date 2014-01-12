using System;
using System.Collections.Generic;
using System.Linq;
using Autofac.Analysis.Transport.Model;

namespace Autofac.Analysis.Engine.Application
{
    public class Component : IApplicationItem
    {
        readonly string _id;
        readonly TypeData _limitType;
        readonly IEnumerable<Service> _services;
        readonly OwnershipModel _ownership;
        readonly SharingModel _sharing;
        readonly IDictionary<string, string> _metadata;
        readonly ActivatorModel _activator;
        readonly LifetimeModel _lifetime;
        readonly string _targetComponentId;

        public Component(string id, TypeData limitType, IEnumerable<Service> services, OwnershipModel ownership, SharingModel sharing, IDictionary<string, string> metadata, ActivatorModel activator, LifetimeModel lifetime, string targetComponentId = null)
        {
            if (limitType == null) throw new ArgumentNullException("limitType");
            if (services == null) throw new ArgumentNullException("services");
            if (metadata == null) throw new ArgumentNullException("metadata");
            _id = id;
            _limitType = limitType;
            _services = services.ToArray();
            _ownership = ownership;
            _sharing = sharing;
            _metadata = metadata;
            _activator = activator;
            _lifetime = lifetime;
            _targetComponentId = targetComponentId;
        }

        public string TargetComponentId
        {
            get { return _targetComponentId; }
        }

        public LifetimeModel Lifetime
        {
            get { return _lifetime; }
        }

        public ActivatorModel Activator
        {
            get { return _activator; }
        }

        public SharingModel Sharing
        {
            get { return _sharing; }
        }

        public OwnershipModel Ownership
        {
            get { return _ownership; }
        }

        public IEnumerable<Service> Services
        {
            get { return _services; }
        }

        public TypeData LimitType
        {
            get { return _limitType; }
        }

        public string Id { get { return _id; } }

        public string Description
        {
            get
            {
                if (LimitType.Identity.DisplayName != typeof(object).Name)
                    return LimitType.Identity.DisplayFullName;

                return "Unknown (" + string.Join(", ", Services.Select(s => s.ServiceType.Identity.DisplayName)) + ")";
            }
        }

        public bool IsTracked
        {
            get { return LimitType.IsDisposable && Ownership == OwnershipModel.OwnedByLifetimeScope; }
        }

        public IDictionary<string, string> Metadata
        {
            get { return _metadata; }
        }
    }
}
