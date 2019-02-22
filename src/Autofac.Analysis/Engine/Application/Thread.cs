using System;
using System.Collections.Generic;

namespace Autofac.Analysis.Engine.Application
{
    public class Thread : IApplicationItem
    {
        public Thread(string id)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
        }

        public string Id { get; }

        public Stack<ResolveOperation> ResolveOperationStack { get; } = new Stack<ResolveOperation>();
    }
}
