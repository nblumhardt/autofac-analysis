using System;
using Serilog.Events;

namespace Autofac.Analysis.Engine.Analytics
{
    public class MessageEvent
    {
        readonly LogEventLevel _level;
        readonly string _messageTemplate;
        readonly object[] _args;

        public MessageEvent(LogEventLevel level, string messageTemplate, params object[] args)
        {
            if (messageTemplate == null) throw new ArgumentNullException("messageTemplate");
            if (args == null) throw new ArgumentNullException("args");
            _level = level;
            _messageTemplate = messageTemplate;
            _args = args;
        }

        public LogEventLevel Level
        {
            get { return _level; }
        }

        public string MessageTemplate
        {
            get { return _messageTemplate; }
        }

        public object[] Args
        {
            get { return _args; }
        }
    }
}
