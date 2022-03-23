namespace ReconNess.Core.Models
{
    public class AgentRunnerQueue
    {
        public AgentRunner AgentRunner { get; set; }
        public string RunnerId { get; set; }
        public string Command { get; set; }
        public string AgentRunnerType { get; set; }
        public bool Last { get; set; }
        public bool AllowSkip { get; set; }
    }
}
