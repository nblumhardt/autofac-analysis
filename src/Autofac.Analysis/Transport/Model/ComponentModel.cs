using System;
using System.Collections.Generic;
using System.Linq;

namespace Autofac.Analysis.Transport.Model
{
    [Serializable]
    public class ComponentModel
    {
        readonly string _targetComponentId;
        readonly OwnershipModel _ownership;
        readonly SharingModel _sharing;
        readonly LifetimeModel _lifetime;
        readonly ActivatorModel _activator;
        readonly string _id;
        readonly string _limitTypeId;
        readonly IDictionary<string, string> _metadata;
        readonly IEnumerable<ServiceModel> _services;

        public ComponentModel(
            string id,
            IEnumerable<ServiceModel> services,
            string limitTypeId,
            IDictionary<string, string> metadata,
            string targetComponentId,
            OwnershipModel ownership,
            SharingModel sharing,
            LifetimeModel lifetime,
            ActivatorModel activator)
        {
            if (id == null) throw new ArgumentNullException("id");
            if (services == null) throw new ArgumentNullException("services");
            if (limitTypeId == null) throw new ArgumentNullException("limitTypeId");
            if (metadata == null) throw new ArgumentNullException("metadata");
            if (targetComponentId == null) throw new ArgumentNullException("targetComponentId");
            _id = id;
            _limitTypeId = limitTypeId;
            _metadata = new Dictionary<string,string>(metadata);
            _services = services.ToArray();
            _targetComponentId = targetComponentId;
            _ownership = ownership;
            _sharing = sharing;
            _lifetime = lifetime;
            _activator = activator;
        }

        public ActivatorModel Activator
        {
            get { return _activator; }
        }

        public LifetimeModel Lifetime
        {
            get { return _lifetime; }
        }

        public SharingModel Sharing
        {
            get { return _sharing; }
        }

        public OwnershipModel Ownership
        {
            get { return _ownership; }
        }

        public string LimitTypeId
        {
            get { return _limitTypeId; }
        }

        public IDictionary<string, string> Metadata
        {
            get { return _metadata; }
        }

        public IEnumerable<ServiceModel> Services
        {
            get { return _services; }
        }

        public string Id
        {
            get { return _id; }
        }

        public string TargetComponentId
        {
            get { return _targetComponentId; }
        }
    }
}
