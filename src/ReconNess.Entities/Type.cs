using System;
using System.Collections.Generic;

namespace ReconNess.Entities
{
    public class Type : BaseEntity, IEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<AgentType> AgentTypes { get; set; }
    }
}
