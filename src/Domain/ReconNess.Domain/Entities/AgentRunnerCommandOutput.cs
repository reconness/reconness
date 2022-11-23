using System;

namespace ReconNess.Domain.Entities;

public class AgentRunnerCommandOutput : BaseEntity, IEntity
{
    public Guid Id { get; set; }

    public string Output { get; set; }

    public virtual AgentRunnerCommand AgentRunnerCommand { get; set; }
}
