using System;
using System.Collections.Generic;

namespace ReconNess.Entities
{
    public class ServiceHttp : BaseEntity, IEntity
    {
        public Guid Id { get; set; }

        public string ScreenshotHttpPNGBase64 { get; set; }

        public string ScreenshotHttpsPNGBase64 { get; set; }

        public virtual ICollection<ServiceHttpDirectory> Directories { get; set; }

        public virtual ICollection<Subdomain> Subdomains { get; set; }
    }
}
