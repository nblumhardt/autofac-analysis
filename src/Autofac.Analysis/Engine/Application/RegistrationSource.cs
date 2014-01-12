using System;

namespace Autofac.Analysis.Engine.Application
{
    public class RegistrationSource : IApplicationItem
    {
        readonly string _id;
        readonly TypeIdentifier _typeIdentifier;
        readonly string _description;

        public RegistrationSource(string id, TypeIdentifier typeIdentifier, string description)
        {
            if (id == null) throw new ArgumentNullException("id");
            if (typeIdentifier == null) throw new ArgumentNullException("typeIdentifier");
            if (description == null) throw new ArgumentNullException("description");
            _id = id;
            _typeIdentifier = typeIdentifier;
            _description = description;
        }

        public TypeIdentifier TypeIdentifier
        {
            get { return _typeIdentifier; }
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
