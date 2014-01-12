using System;

namespace Autofac.Analysis.Transport.Model
{
    [Serializable]
    public class ServiceModel
    {
        readonly string _key;
        readonly string _serviceTypeId;
        readonly string _description;

        public ServiceModel(string key = null, string serviceTypeId = null, string description = null)
        {
            _key = key;
            _serviceTypeId = serviceTypeId;
            _description = description;
        }

        public string Description
        {
            get { return _description; }
        }

        public string ServiceTypeId
        {
            get { return _serviceTypeId; }
        }

        public string Key
        {
            get { return _key; }
        }

        public override bool Equals(object obj)
        {
            var that = obj as ServiceModel;
            if (that == null)
                return false;

            return AsTuple().Equals(that.AsTuple());
        }

        public override int GetHashCode()
        {
            return AsTuple().GetHashCode();
        }

        Tuple<string, string, string> AsTuple() { return Tuple.Create(_key, _serviceTypeId, _description); }
    }
}
