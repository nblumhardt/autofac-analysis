using System;
using System.Collections.Generic;

namespace Autofac.Analysis.Engine.Application
{
    public class LifetimeScope : IApplicationItem
    {
        readonly string _id;
        readonly string _tag;
        readonly LifetimeScope _parent;
        readonly ICollection<LifetimeScope> _activeChildren = new HashSet<LifetimeScope>();

        public LifetimeScope(string id, string tag = null, LifetimeScope parent = null)
        {
            if (id == null) throw new ArgumentNullException("id");
            _id = id;
            _tag = tag;
            _parent = parent;
        }

        public string Tag
        {
            get { return _tag; }
        }

        public LifetimeScope Parent
        {
            get { return _parent; }
        }

        public string Id
        {
            get { return _id; }
        }

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
            get { return _parent == null; }
        }

        public ICollection<LifetimeScope> ActiveChildren
        {
            get { return _activeChildren; }
        }
    }
}
