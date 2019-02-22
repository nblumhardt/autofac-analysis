using System;

namespace Autofac.Analysis.Transport.Model
{
    public class ParameterModel
    {
        public ParameterModel(string description)
        {
            Description = description ?? throw new ArgumentNullException(nameof(description));
        }

        public string Description { get; }
    }
}
