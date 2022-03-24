namespace ReconNess.Core.Models
{
    public class AgentRunnerQueue
    {
        public string Channel { get; set; }
        public string Command { get; set; }
        public string AgentRunnerType { get; set; }
        public bool Last { get; set; }
        public bool AllowSkip { get; set; }
    }
}
