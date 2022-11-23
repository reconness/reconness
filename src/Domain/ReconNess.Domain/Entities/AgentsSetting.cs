using ReconNess.Domain.Enum;
using System;

namespace ReconNess.Domain.Entities;

public class AgentsSetting : BaseEntity, IEntity
{
    public Guid Id { get; set; }

    public AgentRunnerStrategy Strategy { get; set; } = AgentRunnerStrategy.ROUND_ROBIN;

    public int AgentServerCount { get; set; } = 1;
}
