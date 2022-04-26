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
        /// Obtain all the Agents with not tracking
        /// </summary>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>List of Agents with categories or null</returns>
        Task<List<Agent>> GetAgentsNoTrackingAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtain an Agent with not tracking
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A Agent or null</returns>
        Task<Agent> GetAgentNoTrackingAsync(Expression<Func<Agent, bool>> criteria, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtain an Agent
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A Agent or null</returns>
        Task<Agent> GetAgentAsync(Expression<Func<Agent, bool>> criteria, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtain an Agent to Run
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A Agent or null</returns>
        Task<Agent> GetAgentToRunAsync(Expression<Func<Agent, bool>> criteria, CancellationToken cancellationToken = default);

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
        Task<string> GetScriptAsync(string scriptUrl, CancellationToken cancellationToken = default);

        /// <summary>
        /// Read the configuration file
        /// </summary>
        /// <param name="configurationFileName">The configuration file name</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns></returns>
        Task<string> ReadConfigurationFileAsync(string configurationFileName, CancellationToken cancellationToken);

        /// <summary>
        /// Update configuration file
        /// </summary>
        /// <param name="agent">The agent</param>
        /// <param name="configurationContent">The new content</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns></returns>
        Task UpdateConfigurationFileAsync(Agent agent, string configurationContent, CancellationToken cancellationToken);

        /// <summary>
        /// Delete configuration file
        /// </summary>
        /// <param name="agent">The agent</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns></returns>
        Task DeleteConfigurationFileAsync(Agent agent, CancellationToken cancellationToken);
        
    }
}
