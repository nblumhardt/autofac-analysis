using System;
using Autofac.Analysis.Transport.Model;

namespace Autofac.Analysis.Transport.Messages
{
    [Serializable]
    public class ComponentAddedMessage
    {
        readonly ComponentModel _component;

        public ComponentAddedMessage(ComponentModel component)
        {
            _component = component;
        }

        public ComponentModel Component
        {
            get { return _component; }
        }
    }
}
