using System;

namespace ReconNess.Entities
{
    public class AgentLog : BaseEntity, IEntity
    {
        public Guid Id { get; set; }

        public string Log { get; set; }

        public string Username { get; set; }

        public virtual Agent Agent { get; set; }
    }
}
