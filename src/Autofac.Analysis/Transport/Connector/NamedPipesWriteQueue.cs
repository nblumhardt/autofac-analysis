using System;
using System.IO;
using System.IO.Pipes;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Autofac.Analysis.Transport.Util;

namespace Autofac.Analysis.Transport.Connector
{
    public sealed class NamedPipesWriteQueue : Disposable, IWriteQueue
    {
        IWriteQueue _writeQueue;
        IReadQueue _readQueue;
        readonly NamedPipeClientStream _clientStream;
        readonly Timer _timer;
        readonly BinaryFormatter _formatter = new BinaryFormatter();
        readonly DateTime _initTime;
        readonly TimeSpan _giveUpInterval = TimeSpan.FromMinutes(10);
        readonly TimeSpan _connectionRetryInterval = TimeSpan.FromMilliseconds(500);

        public NamedPipesWriteQueue()
        {
            _clientStream = new NamedPipeClientStream(Constants.ThisMachineServerName, Constants.PipeName, PipeDirection.Out);
            _timer = new Timer(WriteToQueue);
            _initTime = DateTime.Now;

            var inProcQueue = new InProcQueue();
            _readQueue = inProcQueue;
            _writeQueue = inProcQueue;

            Trace("Starting up.");

            ResetTimer();
        }

        public void Enqueue(object message)
        {
            // Since we eagerly dispose ourselves after failing to connect
            // to the server, we don't check for disposal here but rather
            // perform a no-op by using a NullQueue.
            _writeQueue.Enqueue(message);
        }

        void WriteToQueue(object state)
        {
            try
            {
                if (!_clientStream.IsConnected)
                    _clientStream.Connect((int)_connectionRetryInterval.TotalMilliseconds);

                object message;
                while (_readQueue.TryDequeue(out message))
                {
                    var ms = new MemoryStream();
                    _formatter.Serialize(ms, message);
                    var bytes = ms.ToArray();
                    _clientStream.Write(bytes, 0, bytes.Length);
                    _clientStream.Flush();
                }

                ResetTimer();
            }
            catch (TimeoutException timeoutException)
            {
                Trace("Could not connect to profiler: {0}", timeoutException.Message);
                if (DateTime.Now - _initTime < _giveUpInterval)
                {
                    ResetTimer();
                }
                else
                {
                    Trace("Connection attempt threshold exceeded; detaching event queue and giving up.");
                    var nullQueue = new NullQueue();
                    _readQueue = nullQueue;
                    _writeQueue = nullQueue;
                    Dispose();
                }
            }
            catch (Exception exception)
            {
                Trace("Error communicating with profiler, giving up: {0}", exception.Message);
            }
        }

        void ResetTimer()
        {
            _timer.Change((int)_connectionRetryInterval.TotalMilliseconds, Timeout.Infinite);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _timer.Dispose();
                _clientStream.Dispose();
            }

            base.Dispose(disposing);
        }

        static void Trace(string message, params object[] formatArgs)
        {
            var traceLine = "[Whitebox.Connector.NamedPipesWriteQueue] " + string.Format(message, formatArgs);
            System.Diagnostics.Trace.WriteLine(traceLine);
        }
    }
}
