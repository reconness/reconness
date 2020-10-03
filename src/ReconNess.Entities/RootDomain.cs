using System;
using System.Collections.Generic;

namespace ReconNess.Entities
{
    public class RootDomain : BaseEntity, IEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string AgentsRanBefore { get; set; }

        public bool HasBounty { get; set; }

        public virtual ICollection<Subdomain> Subdomains { get; set; }

        public virtual Note Notes { get; set; }

        public virtual Target Target { get; set; }
    }
}
