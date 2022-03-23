using System;

namespace ReconNess.Entities
{
    public class AgentRunnerOutput : BaseEntity, IEntity
    {
        public Guid Id { get; set; }

        public string Output { get; set; }

        public virtual AgentRunner AgentRunner { get; set; }
    }
}
