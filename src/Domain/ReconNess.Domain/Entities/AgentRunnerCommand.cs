using ReconNess.Domain.Enum;
using System;
using System.Collections.Generic;

namespace ReconNess.Domain.Entities;

public class AgentRunnerCommand : BaseEntity, IEntity
{
    public Guid Id { get; set; }

    public AgentRunnerCommandStatus Status { get; set; }

    public string Command { get; set; }

    public int Number { get; set; }

    public int Server { get; set; }

    public string Error { get; set; }

    public virtual AgentRunner AgentRunner { get; set; }

    public virtual ICollection<AgentRunnerCommandOutput> Outputs { get; set; }
}
