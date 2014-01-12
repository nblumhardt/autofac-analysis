using System;

namespace Autofac.Analysis.Transport.Model
{
    [Serializable]
    public class LifetimeScopeModel
    {
        readonly string _id;
        readonly string _tag;
        readonly string _parentLifetimeScopeId;

        public LifetimeScopeModel(string id, string tag, string parentLifetimeScopeId = null)
        {
            if (id == null) throw new ArgumentNullException("id");
            if (tag == null) throw new ArgumentNullException("tag");
            _id = id;
            _parentLifetimeScopeId = parentLifetimeScopeId;
            _tag = tag;
        }

        public string Tag
        {
            get { return _tag; }
        }

        public string Id
        {
            get { return _id; }
        }


        public string ParentLifetimeScopeId
        {
            get { return _parentLifetimeScopeId; }
        }
    }
}
