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
        /// <param name="agentRun">The agent run parameters</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        Task RunAsync(AgentRun agentRun, CancellationToken cancellationToken = default);

        /// <summary>
        /// Stop the agent if it is running
        /// </summary>
        /// <param name="agentRun">The agent run parameters</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        Task StopAsync(AgentRun agentRun, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtain a list of agents name that still are running
        /// </summary>
        /// <param name="agentRun">The agent run parameters</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A list of agents name that still are running</returns>
        Task<List<string>> RunningAsync(AgentRun agentRun, CancellationToken cancellationToken = default);
    }
}
