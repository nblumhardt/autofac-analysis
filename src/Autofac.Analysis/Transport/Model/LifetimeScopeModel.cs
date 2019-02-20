using System;

namespace Autofac.Analysis.Transport.Model
{
    public class LifetimeScopeModel
    {
        public LifetimeScopeModel(string id, string tag, string parentLifetimeScopeId = null)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            ParentLifetimeScopeId = parentLifetimeScopeId;
            Tag = tag ?? throw new ArgumentNullException(nameof(tag));
        }

        public string Tag { get; }

        public string Id { get; }


        public string ParentLifetimeScopeId { get; }
    }
}
