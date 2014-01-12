using Autofac.Analysis.Transport.Connector;

namespace Autofac.Analysis.Engine.Session
{
    interface IMessageDispatcher
    {
        void DispatchMessages(IReadQueue readQueue);
    }
}
