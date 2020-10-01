using System;

namespace ReconNess.Entities
{
    public class AgentType : BaseEntity, IEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        
        public virtual Agent Agent { get; set; }
    }
}
