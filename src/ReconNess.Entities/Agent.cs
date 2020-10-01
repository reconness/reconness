using System;
using System.Collections.Generic;

namespace ReconNess.Entities
{
    public class Agent : BaseEntity, IEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Command { get; set; }

        public string Repository { get; set; }

        public string Script { get; set; }

        public DateTime? LastRun { get; set; }

        public virtual AgentTrigger AgentTrigger { get; set; }

        public virtual AgentType AgentType { get; set; }

        public Guid? AgentTypeId { get; set; }

        public virtual ICollection<AgentCategory> AgentCategories { get; set; }

        public virtual ICollection<AgentHistory> AgentHistories { get; set; }

        public virtual ICollection<AgentRun> AgentRuns { get; set; }
    }
}
