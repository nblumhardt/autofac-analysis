namespace Autofac.Analysis.Transport.Connector
{
    public interface IWriteQueue
    {
        void Enqueue(object message);
    }
}