namespace ReconNess.Core.Models
{
    public class AgentRunnerQueue
    {
        public string Channel { get; set; }        
        public string Command { get; set; }
        public int Count { get; set; }
        public int AvailableServerNumber { get; set; }
    }
}
