
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Worker.Models
{
    public class AgentRunnerProcess
    {
        public AgentRunnerProcess(string channel, ProcessWrapper processWrapper, Func<CancellationToken, Task> processFunc)
        {
            Channel = channel;
            ProcessWrapper = processWrapper;
            ProcessFunc = processFunc;
        }

        public string Channel { get; set; }

        public ProcessWrapper ProcessWrapper { get; set; }

        public Func<CancellationToken, Task> ProcessFunc { get; set; }
    }
}
