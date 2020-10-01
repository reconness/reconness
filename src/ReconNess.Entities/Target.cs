using System;
using System.Collections.Generic;

namespace ReconNess.Entities
{
    public class Target : BaseEntity, IEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string AgentsRawBefore { get; set; }

        public string InScope { get; set; }

        public string OutOfScope { get; set; }

        public string BugBountyProgramUrl { get; set; }

        public bool IsPrivate { get; set; }

        public virtual ICollection<RootDomain> RootDomains { get; set; }
    }
}
