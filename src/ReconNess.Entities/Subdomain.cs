using System;
using System.Collections.Generic;

namespace ReconNess.Entities
{
    public class Subdomain : BaseEntity, IEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public virtual RootDomain Domain { get; set; }

        public bool? HasHttpOpen { get; set; }

        public bool? IsMainPortal { get; set; }

        public bool? Takeover { get; set; }

        public bool? IsAlive { get; set; }

        public string FromAgents { get; set; }

        public string IpAddress { get; set; }

        public virtual ICollection<SubdomainLabel> Labels { get; set; }

        public virtual Note Notes { get; set; }

        public virtual ServiceHttp ServiceHttp { get; set; }

        public virtual ICollection<Service> Services { get; set; }
    }
}
