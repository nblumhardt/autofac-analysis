using System;

namespace Autofac.Analysis.Engine.Application
{
    public class Service
    {
        readonly string _description;
        readonly TypeData _serviceType;
        readonly string _key;
        
        public Service(string description, TypeData serviceType = null, string key = null)
        {
            if (description == null) throw new ArgumentNullException("description");
            _serviceType = serviceType;
            _key = key;

            if (serviceType == null)
                _description = description;
            else
            {
                _description = serviceType.Identity.DisplayName;
                if (key != null)
                {
                    _description += " (" + key + ")";
                }
            }
        }

        public bool IsTypedService { get { return _key == null && _serviceType != null; } }

        public string Key
        {
            get { return _key; }
        }

        public string Description
        {
            get { return _description; }
        }

        public TypeData ServiceType
        {
            get { return _serviceType; }
        }
    }
}
