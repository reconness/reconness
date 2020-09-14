using System;

namespace ReconNess.Entities
{
    public class AgentHistory : BaseEntity, IEntity
    {
        public Guid Id { get; set; }

        public string ChangeType { get; set; }

        public string Username { get; set; }

        public virtual Agent Agent { get; set; }
    }
}
