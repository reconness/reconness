using ReconNess.Entities.Enum;
using System;

namespace ReconNess.Entities
{
    public class AgentsSetting : BaseEntity, IEntity
    {
        public Guid Id { get; set; }

        public AgentRunnerStrategy Strategy { get; set; } = AgentRunnerStrategy.ROUND_ROBIN;

        public int AgentServerCount { get; set; } = 1;
    }
}
