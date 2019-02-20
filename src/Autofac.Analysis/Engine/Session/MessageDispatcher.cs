using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac.Analysis.Transport.Connector;

namespace Autofac.Analysis.Engine.Session
{
    class MessageDispatcher : IMessageDispatcher
    {
        readonly IComponentContext _componentContext;
        readonly IApplicationEventDispatcher _applicationEventDispatcher;
        static readonly MethodInfo DispatchMessageOfTypeMethod = typeof (MessageDispatcher).GetMethod("DispatchMessageOfType", BindingFlags.NonPublic | BindingFlags.Instance);

        public MessageDispatcher(IComponentContext componentContext, IApplicationEventDispatcher applicationEventDispatcher)
        {
            _componentContext = componentContext ?? throw new ArgumentNullException(nameof(componentContext));
            _applicationEventDispatcher = applicationEventDispatcher ?? throw new ArgumentNullException(nameof(applicationEventDispatcher));
        }

        public void DispatchMessages(IReadQueue readQueue)
        {
            if (readQueue == null) throw new ArgumentNullException(nameof(readQueue));

            while (readQueue.TryDequeue(out var message))
            {
                var messageType = message.GetType();
                var dispatchMethod = DispatchMessageOfTypeMethod.MakeGenericMethod(messageType);
                dispatchMethod.Invoke(this, new[] { message });
                _applicationEventDispatcher.DispatchApplicationEvents();
            }

            _applicationEventDispatcher.DispatchApplicationEvents();
        }

        // ReSharper disable UnusedMember.Local
        void DispatchMessageOfType<TMessage>(TMessage message)
        // ReSharper restore UnusedMember.Local
        {
            foreach (var handler in _componentContext.Resolve<IEnumerable<IUpdateHandler<TMessage>>>())
                handler.UpdateFrom(message);
        }
    }
}
