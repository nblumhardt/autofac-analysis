using System;

namespace Autofac.Analysis.Engine.Application
{
    public class Service
    {
        public Service(string description, Type serviceType = null, object key = null)
        {
            if (description == null) throw new ArgumentNullException(nameof(description));
            ServiceType = serviceType;
            Key = key;

            if (serviceType == null)
                Description = description;
            else
            {
                Description = serviceType.ToString();
                if (key != null)
                {
                    Description += " (" + key + ")";
                }
            }
        }

        public object Key { get; }

        public string Description { get; }

        public Type ServiceType { get; }
    }
}
