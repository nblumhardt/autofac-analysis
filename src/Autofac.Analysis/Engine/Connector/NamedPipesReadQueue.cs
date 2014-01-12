using System;
using System.IO;
using System.IO.Pipes;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.AccessControl;
using Autofac.Analysis.Transport.Connector;
using Autofac.Analysis.Transport.Util;

namespace Autofac.Analysis.Engine.Connector
{
    public sealed class NamedPipesReadQueue : Disposable, IReadQueue
    {
        const int MaxMessage = 4096;
        const int MaxMessagesInBuffer = 100;
        const int MaxInstances = 10;

        readonly InProcQueue _inProcQueue = new InProcQueue();
        readonly NamedPipeServerStream _serverStream;
        readonly byte[] _readBuffer = new byte[MaxMessage];
        readonly BinaryFormatter _formatter = new BinaryFormatter();
        volatile bool _isConnected;

        public NamedPipesReadQueue()
        {
            var ps = new PipeSecurity();
            ps.AddAccessRule(new PipeAccessRule("Users", PipeAccessRights.ReadWrite | PipeAccessRights.CreateNewInstance, AccessControlType.Allow));
            _serverStream = new NamedPipeServerStream(Transport.Connector.Constants.PipeName, PipeDirection.In, MaxInstances,
                                                      PipeTransmissionMode.Message, PipeOptions.None, MaxMessage * MaxMessagesInBuffer,
                                                      MaxMessage, ps);
        }

        public void WaitForConnection()
        {
            _serverStream.WaitForConnection();
            _isConnected = true;
            ReadFromQueue();
        }

        public bool IsConnected
        {
            get { return _isConnected; }
        }

        public bool TryDequeue(out object message)
        {
            return _inProcQueue.TryDequeue(out message);
        }

        void ReadFromQueue()
        {
            if (!_serverStream.IsConnected)
                return;

            _serverStream.BeginRead(_readBuffer, 0, MaxMessage, ar =>
            {
                try
                {                    
                    var bytes = _serverStream.EndRead(ar);
                    if (bytes == 0)
                    {
                        Trace("Connection closed.");
                        _isConnected = false;
                        return;
                    }

                    var message = _formatter.Deserialize(new MemoryStream(_readBuffer));
                    _inProcQueue.Enqueue(message);
                    ReadFromQueue();
                }
                catch (Exception ex)
                {
                    _isConnected = false;
                    Trace("Connection closed with error: {0}.", ex.Message);
                }
            }, null);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _serverStream.Close();
                _serverStream.Dispose();
                _isConnected = false;
            }
        }

        static void Trace(string message, params object[] formatArgs)
        {
            var traceLine = "[Whitebox.Core.Connector.NamedPipesReadQueue] " + string.Format(message, formatArgs);
            System.Diagnostics.Trace.WriteLine(traceLine);
        }
    }
}
