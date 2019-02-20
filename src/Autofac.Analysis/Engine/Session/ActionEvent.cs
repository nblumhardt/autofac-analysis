using System;

namespace Autofac.Analysis.Engine.Session
{
    class ActionEvent
    {
        public ActionEvent(Action action)
        {
            Action = action ?? throw new ArgumentNullException(nameof(action));
        }

        public Action Action { get; }
    }
}
