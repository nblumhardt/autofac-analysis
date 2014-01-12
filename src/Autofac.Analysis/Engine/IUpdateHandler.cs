namespace Autofac.Analysis.Engine
{
    public interface IUpdateHandler<in TMessage>
    {
        void UpdateFrom(TMessage message);
    }
}
