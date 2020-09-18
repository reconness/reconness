
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Worker.Models
{
    public class AgentRunnerProcess
    {
        public AgentRunnerProcess(string key, ProcessWrapper processWrapper, Func<CancellationToken, Task> processFunc)
        {
            Key = key;
            ProcessWrapper = processWrapper;
            ProcessFunc = processFunc;
        }

        public string Key { get; set; }

        public ProcessWrapper ProcessWrapper { get; set; }

        public Func<CancellationToken, Task> ProcessFunc { get; set; }
    }
}
