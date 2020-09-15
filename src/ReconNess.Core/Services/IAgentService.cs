using ReconNess.Core.Models;
using ReconNess.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

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
        Task<List<Agent>> GetAllWithIncludeAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtain an Agent with categories
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A Agent or null</returns>
        Task<Agent> GetWithIncludeAsync(Expression<Func<Agent, bool>> criteria, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtain default Agents to install
        /// </summary>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>List of default Agents</returns>
        Task<List<AgentMarketplace>> GetMarketplaceAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Allow Debug a script with a terminal output provide manually
        /// </summary>
        /// <param name="script">The script</param>
        /// <param name="terminalOutput">Terminal output</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A ScriptOutput class or Exception</returns>
        Task<ScriptOutput> DebugAsync(string script, string terminalOutput, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtain the Agent Script using the URL
        /// </summary>
        /// <param name="scriptUrl">The URL where we have the script</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The Agent Script using the URL</returns>
        Task<string> GetScriptAsync(string scriptUrl, CancellationToken cancellationToken);

        /// <summary>
        /// If we can run this Agent in a Subdomain 
        /// </summary>
        /// <param name="agentName">The agent name</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>If we can run this Agent in a Subdomain </returns>
        Task<bool> IsBySubdomainAsync(string agentName, CancellationToken cancellationToken);
    }
}
