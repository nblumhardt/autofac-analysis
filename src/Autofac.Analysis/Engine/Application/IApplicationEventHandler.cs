namespace Autofac.Analysis.Engine.Application
{
    public interface IApplicationEventHandler<in TEvent>
    {
        void Handle(TEvent applicationEvent);
    }
}
