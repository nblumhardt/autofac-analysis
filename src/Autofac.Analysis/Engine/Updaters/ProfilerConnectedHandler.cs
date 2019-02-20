using System;
using Autofac.Analysis.Engine.Application;
using Autofac.Analysis.Transport.Messages;

namespace Autofac.Analysis.Engine.Updaters
{
    class ProfilerConnectedHandler : IUpdateHandler<ProfilerConnectedMessage>
    {
        readonly IApplicationEventQueue _applicationEventQueue;

        public ProfilerConnectedHandler(IApplicationEventQueue applicationEventQueue)
        {
            _applicationEventQueue = applicationEventQueue;
        }

        public void UpdateFrom(ProfilerConnectedMessage message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            _applicationEventQueue.Enqueue(new ProfilerConnectedEvent(message.ProcessName, message.ProcessId));
        }
    }
}
