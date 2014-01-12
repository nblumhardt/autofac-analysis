using System;
using System.Collections.Generic;

namespace Autofac.Analysis.Engine.Application
{
    public class Thread : IApplicationItem
    {
        readonly string _id;
        readonly Stack<ResolveOperation> _resolveOperationStack = new Stack<ResolveOperation>();

        public Thread(string id)
        {
            if (id == null) throw new ArgumentNullException("id");
            _id = id;
        }

        public string Id
        {
            get { return _id; }
        }

        public Stack<ResolveOperation> ResolveOperationStack
        {
            get { return _resolveOperationStack; }
        }
    }
}
