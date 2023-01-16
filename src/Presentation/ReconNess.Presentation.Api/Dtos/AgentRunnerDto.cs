namespace ReconNess.Presentation.Api.Dtos;

public class AgentRunnerDto
{
    public string RunnerId { get; set; }

    public string Agent { get; set; }

    public string Command { get; set; }

    public string Target { get; set; }

    public string RootDomain { get; set; }

    public string Subdomain { get; set; }

    public bool ActivateNotification { get; set; }
}
