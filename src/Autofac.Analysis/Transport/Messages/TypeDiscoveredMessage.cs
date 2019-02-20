using System;
using Autofac.Analysis.Transport.Model;

namespace Autofac.Analysis.Transport.Messages
{
    [Serializable]
    public class TypeDiscoveredMessage
    {
        readonly TypeModel _typeModel;

        public TypeDiscoveredMessage(TypeModel typeModel)
        {
            if (typeModel == null) throw new ArgumentNullException(nameof(typeModel));
            _typeModel = typeModel;
        }

        public TypeModel TypeModel
        {
            get { return _typeModel; }
        }
    }
}
