using System;
using System.Collections.Generic;

namespace Autofac.Analysis.Engine.Application
{
    public class LifetimeScope : IApplicationItem
    {
        public LifetimeScope(string id, string tag = null, LifetimeScope parent = null)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Tag = tag;
            Parent = parent;
        }

        public string Tag { get; }

        public LifetimeScope Parent { get; }

        public string Id { get; }

        public int Level
        {
            get
            {
                var level = 0;
                var next = this;
                while (next != null)
                {
                    level++;
                    next = next.Parent;
                }
                return level;
            }
        }

        public string Description
        {
            get
            {
                if (Tag == null)
                    return "level " + Level;

                if (Tag == "root")
                    return "root container";

                return Tag;
            }
        }

        public bool IsRootScope
        {
            get { return Parent == null; }
        }

        public ICollection<LifetimeScope> ActiveChildren { get; } = new HashSet<LifetimeScope>();
    }
}
