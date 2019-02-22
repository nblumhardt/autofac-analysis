using System;
using System.Collections.Generic;
using System.Linq;

namespace Autofac.Analysis.Transport.Model
{
    public class InstanceLookupModel
    {
        public InstanceLookupModel(string id, string resolveOperationId, string componentId, string activationScopeId, IEnumerable<ParameterModel> parameters)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            Id = id ?? throw new ArgumentNullException(nameof(id));
            ResolveOperationId = resolveOperationId ?? throw new ArgumentNullException(nameof(resolveOperationId));
            ComponentId = componentId ?? throw new ArgumentNullException(nameof(componentId));
            ActivationScopeId = activationScopeId ?? throw new ArgumentNullException(nameof(activationScopeId));
            Parameters = parameters.ToArray();
        }

        public string Id { get; }

        public string ActivationScopeId { get; }

        public IEnumerable<ParameterModel> Parameters { get; }

        public string ComponentId { get; }

        public string ResolveOperationId { get; }
    }
}
