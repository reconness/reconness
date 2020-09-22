using System;

namespace ReconNess.Entities
{
    public class AgentTrigger : BaseEntity, IEntity
    {
        public Guid Id { get; set; }

        public bool OnlyIfIsAlive { get; set; }

        public bool OnlyIfHasHttpOpen { get; set; }

        public bool SkipIfRanBefore { get; set; }

        public Guid AgentId { get; set; }
        public virtual Agent Agent { get; set; }
    }
}
