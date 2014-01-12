namespace Autofac.Analysis.Engine.Application
{
    public interface IApplicationEventQueue
    {
        void Enqueue(object applicationEvent);
    }
}
