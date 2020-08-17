using System;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Core.Models
{
    public class AgentRunProcess
    {
        public AgentRunProcess(string key, RunnerProcess runnerProcess, Func<CancellationToken, Task> process)
        {
            Key = key;
            RunnerProcess = runnerProcess;
            Process = process;
        }

        public RunnerProcess RunnerProcess { get; set; }

        public string Key { get; set; }

        public bool MarkAsDeleted { get; set; } = false;
        public Func<CancellationToken, Task> Process { get; set; } 
    }
}
