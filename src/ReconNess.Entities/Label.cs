using System;
using System.Collections.Generic;

namespace ReconNess.Entities
{
    public class Label : BaseEntity, IEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<SubdomainLabel> Subdomains { get; set; }
    }
}
