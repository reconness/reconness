using ReconNess.Core.Models;
using ReconNess.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Core.Services
{
    /// <summary>
    /// The interface ISubdomainService
    /// </summary>
    public interface ISubdomainService : IService<Subdomain>
    {
        /// <summary>
        /// Obtain the list of subdomains by target order by CreatedAt desc
        /// </summary>
        /// <param name="target">The target</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The list of subdomains by target order by CreatedAt desc</returns>
        Task<List<Subdomain>> GetSubdomainsByTargetAsync(RootDomain target, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update the subdomain with the output for the terminal
        /// Add IpAddress, if is alive, if HasHttpOpen and Services running on the subdomain
        /// </summary>
        /// <param name="agentRun">The agent raw</param>
        /// <param name="scriptOutput">The terminal output one line</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        Task UpdateSubdomainByAgent(AgentRun agentRun, ScriptOutput scriptOutput, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete the subdomain with the services
        /// </summary>
        /// <param name="subdomain">Subdomain to delete</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A Task</returns>
        Task DeleteSubdomainAsync(Subdomain subdomain, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete all the subdomains with the services
        /// </summary>
        /// <param name="subdomains">The list subdomains to delete</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        void DeleteSubdomains(ICollection<Subdomain> subdomains, CancellationToken cancellationToken = default);
    }
}
