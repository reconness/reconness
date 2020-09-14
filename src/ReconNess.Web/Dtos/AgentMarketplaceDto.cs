namespace ReconNess.Web.Dtos
{
    public class AgentMarketplaceDto
    {
        public string Name { get; set; }

        public string Repository { get; set; }

        public string Category { get; set; }

        public string Command { get; set; }

        public bool IsBySubdomain { get; set; }

        public bool OnlyIfIsAlive { get; set; }

        public bool OnlyIfHasHttpOpen { get; set; }

        public bool SkipIfRanBefore { get; set; }

        public string ScriptUrl { get; set; }
    }
}
