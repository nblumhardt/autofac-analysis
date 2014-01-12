using System.Collections.Concurrent;

namespace Autofac.Analysis.Transport.Connector
{
    public class InProcQueue : IReadQueue, IWriteQueue
    {
        readonly ConcurrentQueue<object> _messages = new ConcurrentQueue<object>();

        public bool TryDequeue(out object message)
        {
            return _messages.TryDequeue(out message);
        }

        public void Enqueue(object message)
        {
            _messages.Enqueue(message);
        }

        public bool IsEmpty
        {
            get
            {
                object peeked;
                return !_messages.TryPeek(out peeked);
            }
        }
    }
}
