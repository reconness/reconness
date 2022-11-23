namespace ReconNess.Application.Models;

public class AgentRunnerQueue
{
    public string Channel { get; set; }  
    
    public string Payload { get; set; }
    public string Command { get; set; }
    public int Number { get; set; }
    public int ServerNumber { get; set; }
}
