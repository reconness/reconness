using System;
using System.Collections.Generic;

namespace ReconNess.Entities
{
    public class Subdomain : BaseEntity, IEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public virtual RootDomain RootDomain { get; set; }

        public bool? IsAlive { get; set; }

        public string IpAddress { get; set; }

        public bool? Takeover { get; set; }

        public bool? HasHttpOpen { get; set; }            

        public bool? IsMainPortal { get; set; }

        public string Technology { get; set; }

        public string ScreenshotHttpPNGBase64 { get; set; }

        public string ScreenshotHttpsPNGBase64 { get; set; }             

        public string AgentsRanBefore { get; set; }                

        public bool? HasBounty { get; set; }

        public virtual ICollection<SubdomainLabel> Labels { get; set; }

        public virtual Note Notes { get; set; }

        public virtual ICollection<Directory> Directories { get; set; }

        public virtual ICollection<Service> Services { get; set; }
    }
}
