using System;
using System.Threading;
using Autofac.Analysis.Engine.Application;
using Autofac.Analysis.Engine.Connector;
using Autofac.Util;

namespace Autofac.Analysis.Engine.Session
{
    // Only Start(), Dispose() and BeginInvoke() are thread-safe
    class ProfilerSession : Disposable, IProfilerSession
    {
        readonly IMessageDispatcher _messageDispatcher;
        readonly IApplicationEventQueue _applicationEventQueue;
        readonly NamedPipesReadQueue _readQueue = new NamedPipesReadQueue();
        readonly Timer _timer;

        const int UpdateIntervalMilliseconds = 100;

        public ProfilerSession(IMessageDispatcher messageDispatcher, IApplicationEventQueue applicationEventQueue)
        {
            if (messageDispatcher == null) throw new ArgumentNullException("messageDispatcher");
            if (applicationEventQueue == null) throw new ArgumentNullException("applicationEventQueue");
            _messageDispatcher = messageDispatcher;
            _applicationEventQueue = applicationEventQueue;
            _timer = new Timer(Update, null, Timeout.Infinite, Timeout.Infinite);
        }

        public event EventHandler<ApplicationConnectedEventArgs> Connected = delegate { };

        public event EventHandler<ApplicationDisconnectedEventArgs> Disconnected = delegate { };

        public void BeginInvoke(Action action)
        {
            if (action == null) throw new ArgumentNullException("action");
            _applicationEventQueue.Enqueue(new ActionEvent(action));
        }

        public void Start()
        {
            var waitThread = new System.Threading.Thread(WaitForConnection);
            waitThread.Start();
        }

        void Update(object state)
        {
            _applicationEventQueue.Enqueue(new ClockTickEvent());
            _messageDispatcher.DispatchMessages(_readQueue);

            if (!_readQueue.IsConnected)
                Disconnected(this, new ApplicationDisconnectedEventArgs());

            _timer.Change(UpdateIntervalMilliseconds, Timeout.Infinite);
        }

        void WaitForConnection(object state)
        {
            _readQueue.WaitForConnection();
            Connected(this, new ApplicationConnectedEventArgs());
            Update(null);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _timer.Dispose();
                _readQueue.Dispose();
            }
        }
    }
}
