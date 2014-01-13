using System;
using System.Threading;
using Autofac.Analysis.Engine.Application;
using Autofac.Analysis.Transport.Connector;
using Autofac.Util;

namespace Autofac.Analysis.Engine.Session
{
    // Only Start(), Dispose() and BeginInvoke() are thread-safe
    class ProfilerSession : Disposable, IProfilerSession
    {
        readonly IMessageDispatcher _messageDispatcher;
        readonly IApplicationEventQueue _applicationEventQueue;
        readonly IReadQueue _readQueue;
        readonly Timer _timer;

        const int UpdateIntervalMilliseconds = 100;

        public ProfilerSession(IMessageDispatcher messageDispatcher, IApplicationEventQueue applicationEventQueue, IReadQueue readQueue)
        {
            if (messageDispatcher == null) throw new ArgumentNullException("messageDispatcher");
            if (applicationEventQueue == null) throw new ArgumentNullException("applicationEventQueue");
            if (readQueue == null) throw new ArgumentNullException("readQueue");
            _messageDispatcher = messageDispatcher;
            _applicationEventQueue = applicationEventQueue;
            _readQueue = readQueue;
            _timer = new Timer(Update, null, Timeout.Infinite, Timeout.Infinite);
        }

        public void BeginInvoke(Action action)
        {
            if (action == null) throw new ArgumentNullException("action");
            _applicationEventQueue.Enqueue(new ActionEvent(action));
        }

        public void Start()
        {
            Update(null);
        }

        void Update(object state)
        {
            _applicationEventQueue.Enqueue(new ClockTickEvent());
            _messageDispatcher.DispatchMessages(_readQueue);

            _timer.Change(UpdateIntervalMilliseconds, Timeout.Infinite);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _timer.Dispose();
            }
        }
    }
}
