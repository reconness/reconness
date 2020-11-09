﻿using ReconNess.Core.Models;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Core.Services
{
    /// <summary>
    /// The interface ICategoryService
    /// </summary>
    public interface IAgentBackgroundService
    {
        /// <summary>
        /// Save the output that the ScriptEnginer returned on database
        /// </summary>
        /// <param name="agentRun">The agent was ran</param>
        /// <param name="agentRunType">The agent run type that was ran</param>
        /// <param name="terminalOutputParse">The output that the ScriptEnginer returned</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A Task</returns>
        Task SaveOutputParseOnScopeAsync(AgentRunner agentRun, string agentRunType, ScriptOutput terminalOutputParse, CancellationToken cancellationToken = default);

        /// <summary>
        /// Register that we run this Agent in the concept
        /// </summary>
        /// <param name="agentRun">The agent</param>
        /// <param name="agentRunType">The agent run type that was ran</param>
        /// <param name="token">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        Task UpdateAgentOnScopeAsync(AgentRunner agentRun, string agentRunType, CancellationToken cancellationToken = default);
    }
}
