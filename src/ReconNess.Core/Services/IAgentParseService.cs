using ReconNess.Core.Models;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Core.Services
{
    /// <summary>
    /// The interface ICategoryService
    /// </summary>
    public interface IAgentParseService
    {
        /// <summary>
        /// Save the output that the ScriptEnginer returned on database
        /// </summary>
        /// <param name="agentRun">The agent was ran</param>
        /// <param name="scriptOutput">The output that the ScriptEnginer returned</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A Task</returns>
        Task SaveScriptOutputAsync(AgentRun agentRun, ScriptOutput scriptOutput, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update the last time that the agent ran
        /// </summary>
        /// <param name="agentRun">The agent</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        Task UpdateLastRunAsync(AgentRun agentRun, CancellationToken cancellationToken = default);
    }
}
