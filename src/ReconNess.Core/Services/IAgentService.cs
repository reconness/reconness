using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ReconNess.Core.Models;
using ReconNess.Entities;

namespace ReconNess.Core.Services
{
    /// <summary>
    /// The interface ICategoryService
    /// </summary>
    public interface IAgentService : IService<Agent>
    {
        /// <summary>
        /// Obtain all the Agents with categories
        /// </summary>
        /// <param name="isBySubdomain">If I need to obtain only the agent that run by subdomain</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>List of Agents with categories or null</returns>
        Task<List<Agent>> GetAllAgentsWithCategoryAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtain an Agent with categories
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A Agent or null</returns>
        Task<Agent> GetAgentWithCategoryAsync(Expression<Func<Agent, bool>> criteria, CancellationToken cancellationToken = default);

        /// <summary>
        /// Run the agent
        /// </summary>
        /// <param name="target">The target</param>
        /// <param name="subdomain">The subdomain</param>
        /// <param name="agent">The agent</param>
        /// <param name="command">The command to run</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        Task RunAsync(Target target, Subdomain subdomain, Agent agent, string command, CancellationToken cancellationToken = default);

        /// <summary>
        /// Stop the agent if it is running
        /// </summary>
        /// <param name="target">The target</param>
        /// <param name="subdomain">The subdomain</param>
        /// <param name="agent">The agent</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        Task StopAsync(Target target, Subdomain subdomain, Agent agent, CancellationToken cancellationToken = default);

        /// <summary>
        /// Allow Debug a script with a terminal output provide manually
        /// </summary>
        /// <param name="terminalOutput">Terminal output</param>
        /// <param name="script">The script</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A ScriptOutput class or Exception</returns>
        Task<ScriptOutput> DebugAsync(string terminalOutput, string script, CancellationToken cancellationToken = default);
    }
}
