﻿using ReconNess.Entities.Enum;
using System;
using System.Collections.Generic;

namespace ReconNess.Entities
{
    public class AgentRunnerCommand : BaseEntity, IEntity
    {
        public Guid Id { get; set; }

        public AgentRunnerCommandStatus Status { get; set; }

        public string Command { get; set; }

        public int Number { get; set; }

        public int Server { get; set; }

        public virtual AgentRunner AgentRunner { get; set; }

        public virtual ICollection<AgentRunnerCommandOutput> Outputs { get; set; }
    }
}
