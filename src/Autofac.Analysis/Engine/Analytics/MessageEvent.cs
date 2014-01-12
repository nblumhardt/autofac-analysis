using System;

namespace Autofac.Analysis.Engine.Analytics
{
    public class MessageEvent
    {
        readonly MessageRelevance _relevance;
        readonly string _title;
        readonly string _message;

        public MessageEvent(MessageRelevance relevance, string title, string message)
        {
            if (title == null) throw new ArgumentNullException("title");
            if (message == null) throw new ArgumentNullException("message");
            _relevance = relevance;
            _title = title;
            _message = message;
        }

        public MessageRelevance Relevance
        {
            get { return _relevance; }
        }

        public string Title
        {
            get { return _title; }
        }

        public string Message
        {
            get { return _message; }
        }
    }
}
