using System;

namespace Autofac.Analysis.Transport.Messages
{
    [Serializable]
    public class ProfilerConnectedMessage
    {
        public ProfilerConnectedMessage(string processName, int processId)
        {
            ProcessName = processName ?? throw new ArgumentNullException(nameof(processName));
            ProcessId = processId;
        }

        public int ProcessId { get; }

        public string ProcessName { get; }
    }
}
