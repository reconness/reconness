using System;
using System.Collections.Generic;

namespace ReconNess.Web.Dtos
{
    public class TargetDto
    {
        public Guid? Id { get; set; }

        public string Name { get; set; }

        public string RootDomain { get; set; }

        public string BugBountyProgramUrl { get; set; }

        public bool IsPrivate { get; set; }

        public string InScope { get; set; }

        public string OutOfScope { get; set; }

        public List<SubdomainDto> Subdomains { get; set; }

        public NoteDto Notes { get; set; }
    }
}
