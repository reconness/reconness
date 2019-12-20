using System;
using System.Collections.Generic;

namespace ReconNess.Entities
{
    public class Target : BaseEntity, IEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string RootDomain { get; set; }

        public string InScope { get; set; }

        public string OutOfScope { get; set; }

        public string BugBountyProgramUrl { get; set; }

        public bool IsPrivate { get; set; }

        public virtual ICollection<Subdomain> Subdomains { get; set; }

        public virtual Note Notes { get; set; }
    }
}
