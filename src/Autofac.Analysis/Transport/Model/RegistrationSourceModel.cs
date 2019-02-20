using System;

namespace Autofac.Analysis.Transport.Model
{
    [Serializable]
    public class RegistrationSourceModel
    {
        readonly string _id;
        readonly string _typeAssemblyQualifiedName;
        readonly string _description;

        public RegistrationSourceModel(string id, string typeAssemblyQualifiedName, string description)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (typeAssemblyQualifiedName == null) throw new ArgumentNullException(nameof(typeAssemblyQualifiedName));
            if (description == null) throw new ArgumentNullException(nameof(description));
            _id = id;
            _typeAssemblyQualifiedName = typeAssemblyQualifiedName;
            _description = description;
        }

        public string TypeAssemblyQualifiedName
        {
            get { return _typeAssemblyQualifiedName; }
        }

        public string Description
        {
            get { return _description; }
        }

        public string Id
        {
            get { return _id; }
        }
    }
}
