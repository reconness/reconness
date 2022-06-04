using System;
using System.Collections.Generic;

namespace ReconNess.Web.Dtos
{
    public class RootDomainDto
    {
        public Guid? Id { get; set; }

        public string Name { get; set; }

        public string AgentsRanBefore { get; set; }

        public List<SubdomainDto> Subdomains { get; set; }
    }
}
