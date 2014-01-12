namespace Autofac.Analysis.Engine.Session
{
    public interface IApplicationEventBus
    {
        void Subscribe(object subscriber);
        void Unsubscribe(object subscriber);
    }
}
