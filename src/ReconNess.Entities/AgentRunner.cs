using ReconNess.Entities.Enum;
using System;
using System.Collections.Generic;

namespace ReconNess.Entities
{
    public class AgentRunner : BaseEntity, IEntity
    {
        public Guid Id { get; set; }

        public string Channel { get; set; }

        public AgentRunStage Stage { get; set; }

        public virtual Agent Agent { get; set; }

        public virtual ICollection<AgentRunnerOutput> Outputs { get; set; }
    }
}
