using ReconNess.Core.Models;
using System.Threading.Tasks;

namespace ReconNess.Core.Providers
{
    /// <summary>
    /// The interface IAgentRunnerProvider
    /// </summary>
    public interface IAgentRunnerQueueProvider
    {
        /// <summary>
        /// Run the Agent
        /// </summary>
        /// <param name="providerArgs">The Agent params</param>
        /// <returns>A task</returns>
        Task EnqueueAsync(AgentRunnerQueue providerArgs);
    }
}