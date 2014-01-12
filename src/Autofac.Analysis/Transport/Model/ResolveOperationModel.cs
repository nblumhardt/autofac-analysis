using System;

namespace Autofac.Analysis.Transport.Model
{
    [Serializable]
    public class ResolveOperationModel
    {
        readonly string _id;
        readonly string _lifetimeScopeId;
        readonly string _threadId;
        readonly string _callingTypeAssemblyQualifiedName;
        readonly string _callingMethodName;

        public ResolveOperationModel(string id, string lifetimeScopeId, string threadId, string callingTypeAssemblyQualifiedName = null, string callingMethodName = null)
        {
            if (id == null) throw new ArgumentNullException("id");
            if (lifetimeScopeId == null) throw new ArgumentNullException("lifetimeScopeId");
            if (threadId == null) throw new ArgumentNullException("threadId");
            _id = id;
            _lifetimeScopeId = lifetimeScopeId;
            _threadId = threadId;
            _callingTypeAssemblyQualifiedName = callingTypeAssemblyQualifiedName;
            _callingMethodName = callingMethodName;
        }

        public string ThreadId
        {
            get { return _threadId; }
        }

        public string CallingMethodName
        {
            get { return _callingMethodName; }
        }

        public string CallingTypeAssemblyQualifiedName
        {
            get { return _callingTypeAssemblyQualifiedName; }
        }

        public string LifetimeScopeId
        {
            get { return _lifetimeScopeId; }
        }

        public string Id
        {
            get { return _id; }
        }
    }
}
