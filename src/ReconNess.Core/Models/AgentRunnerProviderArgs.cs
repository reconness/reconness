using static ReconNess.Core.IAgentRunnerProvider;

namespace ReconNess.Core.Models
{
    public class AgentRunnerProviderArgs
    {
        public string Key { get; set; }
        public AgentRunner AgentRunner { get; set; }
        public string Channel { get; set; }
        public string Command { get; set; }
        public string AgentRunnerType {get; set;}
        public bool Last { get; set; }
        public BeginHandlerAsync BeginHandlerAsync { get; set; }
        public SkipHandlerAsync SkipHandlerAsync {get; set;}
        public ParserOutputHandlerAsync ParserOutputHandlerAsync { get; set; }
        public EndHandlerAsync EndHandlerAsync { get; set; }
        public ExceptionHandlerAsync ExceptionHandlerAsync { get; set; }
    }
}
