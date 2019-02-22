using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac.Analysis.Engine.Application;
using Autofac.Analysis.Engine.Util;

namespace Autofac.Analysis.Engine.Session
{
    // Only the enqueue operation of IApplicationEventQueue is thread-safe
    class ApplicationEventDispatcher : IApplicationEventQueue, IApplicationEventDispatcher, IApplicationEventBus
    {
        readonly IComponentContext _componentContext;
        readonly ConcurrentQueue<object> _events = new ConcurrentQueue<object>();
        static readonly MethodInfo DispatchEventOfTypeMethod = typeof(ApplicationEventDispatcher).GetMethod("DispatchEventOfType", BindingFlags.NonPublic | BindingFlags.Instance);
        readonly ConcurrentList<object> _additionalSubscribers = new ConcurrentList<object>();

        public ApplicationEventDispatcher(IComponentContext componentContext)
        {
            _componentContext = componentContext ?? throw new ArgumentNullException(nameof(componentContext));
        }

        public void Enqueue(object applicationEvent)
        {
            if (applicationEvent == null) throw new ArgumentNullException(nameof(applicationEvent));
            _events.Enqueue(applicationEvent);
        }

        public void DispatchApplicationEvents()
        {
            while (_events.TryDequeue(out var applicationEvent))
            {
                var dispatchMethod = DispatchEventOfTypeMethod.MakeGenericMethod(applicationEvent.GetType());
                dispatchMethod.Invoke(this, new[] { applicationEvent });
            }
        }

        // ReSharper disable UnusedMember.Local
        void DispatchEventOfType<TEvent>(TEvent applicationEvent)
        // ReSharper restore UnusedMember.Local
        {
            var handlers = _componentContext.Resolve<IEnumerable<IApplicationEventHandler<TEvent>>>()
                .Concat(_additionalSubscribers.ToList().OfType<IApplicationEventHandler<TEvent>>());

            foreach (var handler in handlers)
                handler.Handle(applicationEvent);
        }

        public void Subscribe(object subscriber)
        {
            if (subscriber == null) throw new ArgumentNullException(nameof(subscriber));
            _additionalSubscribers.Add(subscriber);
        }

        public void Unsubscribe(object subscriber)
        {
            if (subscriber == null) throw new ArgumentNullException(nameof(subscriber));
            _additionalSubscribers.Remove(subscriber);
        }
    }
}
