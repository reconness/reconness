using ReconNess.Core.Models;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Core.Services
{
    /// <summary>
    /// The interface ISaveTerminalOutputParseService{T}
    /// </summary>
    public interface ISaveTerminalOutputParseService<T>
    {
        /// <summary>
        /// Save the terminal output
        /// </summary>
        /// <param name="type">The concept [target, rootdomain, subdomain]</param>
        /// <param name="agentName">The agent name</param>
        /// <param name="terminalOutputParse">The terminal output parse</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        Task SaveTerminalOutputParseAsync(T type, string agentName, bool activeNotification, ScriptOutput terminalOutputParse, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update the agent name that raw in the concept
        /// </summary>
        /// <param name="type">The concept [target, rootdomain, subdomain]</param>
        /// <param name="agentName">Agent Name</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        Task UpdateAgentRanAsync(T type, string agentName, CancellationToken cancellationToken = default);
    }
}
