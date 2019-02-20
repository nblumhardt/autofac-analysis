using System;
using System.Collections.Generic;
using System.Linq;

namespace Autofac.Analysis.Transport.Model
{
    [Serializable]
    public class InstanceLookupModel
    {
        readonly string _id;
        readonly string _resolveOperationId;
        readonly string _componentId;
        readonly string _activationScopeId;
        readonly IEnumerable<ParameterModel> _parameters;

        public InstanceLookupModel(string id, string resolveOperationId, string componentId, string activationScopeId, IEnumerable<ParameterModel> parameters)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (resolveOperationId == null) throw new ArgumentNullException(nameof(resolveOperationId));
            if (componentId == null) throw new ArgumentNullException(nameof(componentId));
            if (activationScopeId == null) throw new ArgumentNullException(nameof(activationScopeId));
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            _id = id;
            _resolveOperationId = resolveOperationId;
            _componentId = componentId;
            _activationScopeId = activationScopeId;
            _parameters = parameters.ToArray();
        }

        public string Id
        {
            get { return _id; }
        }

        public string ActivationScopeId
        {
            get { return _activationScopeId; }
        }

        public IEnumerable<ParameterModel> Parameters
        {
            get { return _parameters; }
        }

        public string ComponentId
        {
            get { return _componentId; }
        }

        public string ResolveOperationId
        {
            get { return _resolveOperationId; }
        }
    }
}
