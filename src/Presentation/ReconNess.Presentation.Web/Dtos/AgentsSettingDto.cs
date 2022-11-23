namespace ReconNess.Presentation.Api.Dtos;

public class AgentsSettingDto
{
    public string Strategy { get; set; } = "ROUND_ROBIN";

    public int AgentServerCount { get; set; } = 1;
}
