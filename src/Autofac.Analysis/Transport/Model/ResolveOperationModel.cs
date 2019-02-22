using System;
using System.Reflection;

namespace Autofac.Analysis.Transport.Model
{
    public class ResolveOperationModel
    {
        public ResolveOperationModel(string id, string lifetimeScopeId, int managedThreadId, Type callingType = null, MethodBase callingMethod = null)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            LifetimeScopeId = lifetimeScopeId ?? throw new ArgumentNullException(nameof(lifetimeScopeId));
            ManagedThreadId = managedThreadId;
            CallingType = callingType;
            CallingMethod = callingMethod;
        }

        public int ManagedThreadId { get; }

        public MethodBase CallingMethod { get; }

        public Type CallingType { get; }

        public string LifetimeScopeId { get; }

        public string Id { get; }
    }
}
