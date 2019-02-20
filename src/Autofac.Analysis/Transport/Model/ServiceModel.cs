using System;

namespace Autofac.Analysis.Transport.Model
{
    public class ServiceModel
    {
        public ServiceModel(object key = null, Type serviceType = null, string description = null)
        {
            Key = key;
            ServiceType = serviceType;
            Description = description;
        }

        public string Description { get; }

        public Type ServiceType { get; }

        public object Key { get; }

        public override bool Equals(object obj)
        {
            if (!(obj is ServiceModel that))
                return false;

            return AsTuple().Equals(that.AsTuple());
        }

        public override int GetHashCode()
        {
            return AsTuple().GetHashCode();
        }

        Tuple<object, Type, string> AsTuple() { return Tuple.Create(Key, ServiceType, Description); }
    }
}
