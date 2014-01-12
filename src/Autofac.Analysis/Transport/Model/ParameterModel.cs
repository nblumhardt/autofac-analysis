using System;

namespace Autofac.Analysis.Transport.Model
{
    [Serializable]
    public class ParameterModel
    {
        readonly string _description;

        public ParameterModel(string description)
        {
            if (description == null) throw new ArgumentNullException("description");
            _description = description;
        }

        public string Description
        {
            get { return _description; }
        }
    }
}
