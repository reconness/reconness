using ReconNess.Core.Models;
using ReconNess.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Core.Services
{
    /// <summary>
    /// The interface ICategoryService
    /// </summary>
    public interface IAgentScopeService
    {
        /// <summary>
        /// Save the output that the ScriptEnginer returned on database
        /// </summary>
        /// <param name="agentRun">The agent was ran</param>
        /// <param name="terminalOutputParse">The output that the ScriptEnginer returned</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A Task</returns>
        Task SaveTerminalOutputParseOnScopeAsync(AgentRunner agentRun, ScriptOutput terminalOutputParse, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update the last time that the agent ran
        /// </summary>
        /// <param name="agent">The agent</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        Task UpdateLastRunAgentOnScopeAsync(Agent agent, CancellationToken cancellationToken = default);

        /// <summary>
        /// Register that we run this Agent in the Subdomain
        /// </summary>
        /// <param name="agentRun">The agent</param>
        /// <param name="token"></param>
        /// <returns>Notification that operations should be canceled</returns>
        Task UpdateSubdomainAgentOnScopeAsync(AgentRunner agentRun, CancellationToken cancellationToken = default);
    }
}
