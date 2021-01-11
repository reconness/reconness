using ReconNess.Core.Models;
using ReconNess.Core.Providers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReconNess.PubSub
{
    public class QueueAgentRunnerProvider : IAgentRunnerProvider
    {
        public Task<int> RunningCountAsync => throw new NotImplementedException();

        public Task<IList<string>> RunningChannelsAsync => throw new NotImplementedException();

        public Task InitializesAsync(string channel)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsStoppedAsync(string channel)
        {
            throw new NotImplementedException();
        }

        public Task RunAsync(AgentRunnerProviderArgs providerArgs)
        {
            throw new NotImplementedException();
        }

        public Task StopAsync(string channel)
        {
            throw new NotImplementedException();
        }
    }
}
