using System;
using System.Threading;

namespace ReconNess.Core.Models
{
    public class AgentRunnerProviderResult
    {
        public string Key { get; set; }
        public AgentRunner AgentRunner { get; set; }
        public string Channel { get; set; }
        public string Command { get; set; }
        public ScriptOutput ScriptOutput { get; set; }
        public int LineCount { get; set; }
        public string TerminalLineOutput { get; set; }
        public string AgentRunnerType {get; set;}
        public bool Last { get; set; }
        public Exception Exception { get; set; }
        public CancellationToken CancellationToken { get; set; }
    }
}
