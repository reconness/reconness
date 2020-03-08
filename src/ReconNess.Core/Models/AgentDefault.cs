namespace ReconNess.Core.Models
{
    public class AgentDefault
    {
        public string Name { get; set; }

        public string Category { get; set; }

        public string Command { get; set; }

        public bool IsBySubdomain { get; set; }

        public bool OnlyIfIsAlive { get; set; }

        public bool OnlyIfHasHttpOpen { get; set; }

        public bool SkipIfRanBefore { get; set; }

        public string ScriptUrl { get; set; }
    }
}
