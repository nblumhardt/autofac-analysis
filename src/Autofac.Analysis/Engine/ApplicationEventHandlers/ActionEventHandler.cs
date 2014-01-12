using Autofac.Analysis.Engine.Application;
using Autofac.Analysis.Engine.Session;

namespace Autofac.Analysis.Engine.ApplicationEventHandlers
{
    class ActionEventHandler : IApplicationEventHandler<ActionEvent>
    {
        public void Handle(ActionEvent applicationEvent)
        {
            applicationEvent.Action.Invoke();
        }
    }
}
