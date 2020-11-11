using ReconNess.Core.Models;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Core.Services
{
    /// <summary>
    /// The interface IAgentRunService
    /// </summary>
    public interface IAgentRunService
    {
        /// <summary>
        /// Start agent run status for that channel
        /// </summary>
        /// <param name="agentRunner">The agent running</param>
        /// <param name="channel">The channel</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task StartOnScopeAsync(AgentRunner agentRunner, string channel, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update the last agent run status for that channel
        /// </summary>
        /// <param name="agentRunner">The agent running</param>
        /// <param name="channel">The channel</param>
        /// <param name="stoppedManually">If was stopped manually from the UI</param>
        /// <param name="fromException">if was after throw an exception running the agent</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        Task DoneOnScopeAsync(AgentRunner agentRunner, string channel, bool stoppedManually, bool fromException, CancellationToken cancellationToken = default);

        /// <summary>
        /// Insert the terminal output
        /// </summary>
        /// <param name="agentRunner">The agent running</param>
        /// <param name="channel">The channel</param>
        /// <param name="terminalOutput">The terminal output</param>
        /// <param name="includeTime"></param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        Task InsertTerminalScopeAsync(AgentRunner agentRunner, string channel, string terminalOutput, bool includeTime = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Insert the logs
        /// </summary>
        /// <param name="agentRunner">The agent running</param>
        /// <param name="channel">The channel</param>
        /// <param name="logs">The logs</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        Task InsertLogsScopeAsync(AgentRunner agentRunner, string channel, string logs, CancellationToken cancellationToken = default);
    }
}
