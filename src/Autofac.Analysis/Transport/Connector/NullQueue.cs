namespace Autofac.Analysis.Transport.Connector
{
    class NullQueue : IReadQueue, IWriteQueue
    {
        public bool TryDequeue(out object message)
        {
            message = null;
            return false;
        }

        public void Enqueue(object message)
        {
        }
    }
}