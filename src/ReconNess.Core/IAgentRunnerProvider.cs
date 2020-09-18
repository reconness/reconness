using ReconNess.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReconNess.Core
{
    public interface IAgentRunnerProvider
    {
        public delegate Task BeginHandlerAsync(AgentRunnerProviderHandlerArgs args);

        public delegate Task ParserOutputHandlerAsync(AgentRunnerProviderHandlerArgs args);

        public delegate Task EndHandlerAsync(AgentRunnerProviderHandlerArgs args);

        public delegate Task ExceptionHandlerAsync(AgentRunnerProviderHandlerArgs args);

        Task<int> RunningCountAsync { get; }

        Task<IList<string>> RunningKeysAsync { get; }

        Task StopAsync(string key);

        Task<bool> IsStoppedAsync(string key);

        Task RunAsync(AgentRunnerProviderArgs providerArgs);
    }
}