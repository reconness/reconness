using System;
using System.Collections.Generic;

namespace ReconNess.Web.Dtos
{
    public class SubdomainDto
    {
        public Guid? Id { get; set; }

        public string Name { get; set; }

        public string Target { get; set; }

        public string IpAddress { get; set; }

        public bool? Takeover { get; set; }

        public bool? HasHttpOpen { get; set; }

        public bool? IsMainPortal { get; set; }

        public bool? IsAlive { get; set; }

        public string FromAgents { get; set; }

        public NoteDto Notes { get; set; }

        public List<ServiceDto> Services { get; set; }

        public List<string> Labels { get; set; }
    }
}
