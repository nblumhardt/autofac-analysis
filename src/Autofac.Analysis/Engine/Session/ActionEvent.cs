using System;

namespace Autofac.Analysis.Engine.Session
{
    class ActionEvent
    {
        readonly Action _action;

        public ActionEvent(Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            _action = action;
        }

        public Action Action
        {
            get { return _action; }
        }
    }
}
