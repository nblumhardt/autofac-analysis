using System;

namespace Autofac.Analysis.Engine.Application
{
    public class MethodIdentifier
    {
        readonly string _name;
        readonly TypeIdentifier _declaringTypeIdentifier;

        public MethodIdentifier(string name, TypeIdentifier declaringTypeIdentifier)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (declaringTypeIdentifier == null) throw new ArgumentNullException("declaringTypeIdentifier");
            _name = name;
            _declaringTypeIdentifier = declaringTypeIdentifier;
        }

        public TypeIdentifier DeclaringTypeIdentifier
        {
            get { return _declaringTypeIdentifier; }
        }

        public string Name
        {
            get { return _name; }
        }

        public string DisplayName
        {
            get
            {
                if (DeclaringTypeIdentifier.IsAnonymous)
                    return "Anonymous Method";
                return DeclaringTypeIdentifier.DisplayName + "." + Name + "(...)";
            }
        }
    }
}
