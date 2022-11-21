using System;

namespace ReconNess.Core.Models
{
    public class AgentMarketplace
    {
        public Guid Id { get; set;}
        
        public string Name { get; set; }

        public string Repository { get; set; }

        public string Category { get; set; }

        public string Command { get; set; }

        public bool IsByTarget { get; set; }

        public bool IsByRootDomain { get; set; }

        public bool IsBySubdomain { get; set; }

        public bool IsByDirectory { get; set; }

        public bool IsByResource { get; set; }

        public string ScriptUrl { get; set; }
    }
}
