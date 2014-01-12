namespace Autofac.Analysis.Transport.Connector
{
    public interface IReadQueue
    {
        bool TryDequeue(out object message);
    }
}