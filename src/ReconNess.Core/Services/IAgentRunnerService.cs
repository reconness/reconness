using ReconNess.Core.Models;
using ReconNess.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Core.Services
{
    /// <summary>
    /// The interface IAgentRunnerService
    /// </summary>
    public interface IAgentRunnerService : IService<Agent>
    {
        /// <summary>
        /// Run the agent
        /// </summary>
        /// <param name="agentRunner">The agent run parameters</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        Task RunAgentAsync(AgentRunner agentRunner, CancellationToken cancellationToken = default);

        /// <summary>
        /// Stop the agent if it is running
        /// </summary>
        /// <param name="agentRunner">The agent run parameters</param>
        /// <param name="removeSubdomainForTheKey">If we need to remove the subdomain to generate the key</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        Task StopAgentAsync(AgentRunner agentRunner, bool removeSubdomainForTheKey, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtain a list of agents name that still are running
        /// </summary>
        /// <param name="agentRunner">The agent run parameters</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A list of agents name that still are running</returns>        
        Task<List<string>> RunningAgentsAsync(AgentRunner agentRunner, CancellationToken cancellationToken = default);
    }
}
