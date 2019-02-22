using System;
using Serilog.Events;

namespace Autofac.Analysis.Engine.Analytics
{
    public class MessageEvent
    {
        public MessageEvent(LogEventLevel level, string messageTemplate, params object[] args)
        {
            Level = level;
            MessageTemplate = messageTemplate ?? throw new ArgumentNullException(nameof(messageTemplate));
            Args = args ?? throw new ArgumentNullException(nameof(args));
        }

        public LogEventLevel Level { get; }

        public string MessageTemplate { get; }

        public object[] Args { get; }
    }
}
