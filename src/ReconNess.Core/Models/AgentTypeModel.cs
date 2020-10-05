namespace ReconNess.Core.Models
{
    public class AgentTypeModel
    {
        public bool IsByTarget { get; set; }

        public bool IsByRootDomain { get; set; }

        public bool IsBySubdomain { get; set; }

        public bool IsByDirectory { get; set; }

        public bool IsByResource { get; set; }
    }
}
