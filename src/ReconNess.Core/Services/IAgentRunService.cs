using ReconNess.Core.Models;
using ReconNess.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Core.Services
{
    /// <summary>
    /// The interface IAgentRunService
    /// </summary>
    public interface IAgentRunService : IService<AgentRun>
    {
        /// <summary>
        /// Update the last agent run status for that channel
        /// </summary>
        /// <param name="agent">The agent</param>
        /// <param name="channel">The channel</param>
        /// <param name="stoppedManually">If was stopped manually from the UI</param>
        /// <param name="fromException">if was after throw an exception running the agent</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        Task DoneOnScopeAsync(AgentRunner agentRunner, string channel, bool stoppedManually, bool fromException, CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="channel"></param>
        /// <param name="terminalOutput"></param>
        /// <param name="includeTime"></param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        Task InsertTerminalScopeAsync(AgentRunner agentRunner, string channel, string terminalOutput, bool includeTime = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="channel"></param>
        /// <param name="logs"></param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        Task InsertLogsScopeAsync(AgentRunner agentRunner, string channel, string logs, CancellationToken cancellationToken = default);
    }
}
