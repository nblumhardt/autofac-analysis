using System;

namespace Autofac.Analysis.Engine
{
    public interface IProfilerSession
    {
        void Start();
        void BeginInvoke(Action action);
    }
}
