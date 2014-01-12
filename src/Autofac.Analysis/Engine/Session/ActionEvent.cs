using System;

namespace Autofac.Analysis.Engine.Session
{
    class ActionEvent
    {
        readonly Action _action;

        public ActionEvent(Action action)
        {
            if (action == null) throw new ArgumentNullException("action");
            _action = action;
        }

        public Action Action
        {
            get { return _action; }
        }
    }
}
