using System;
using Autofac.Analysis.Transport.Model;

namespace Autofac.Analysis.Transport.Messages
{
    [Serializable]
    public class ComponentAddedMessage
    {
        public ComponentAddedMessage(ComponentModel component)
        {
            Component = component;
        }

        public ComponentModel Component { get; }
    }
}
