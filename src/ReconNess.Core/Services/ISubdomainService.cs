using ReconNess.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Core.Services
{
    /// <summary>
    /// The interface ISubdomainService
    /// </summary>
    public interface ISubdomainService : IService<Subdomain>, ISaveTerminalOutputParseService
    {
        /// <summary>
        /// Obtain the list of subdomains by target order by CreatedAt desc
        /// </summary>
        /// <param name="target">The targer</param>
        /// <param name="rootDomain">The root domain</param>
        /// <param name="subdomain">The subdomain</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The list of subdomains by target order by CreatedAt desc</returns>
        Task<List<Subdomain>> GetSubdomainsAsync(Target target, RootDomain rootDomain, string subdomain, CancellationToken cancellationToken = default);

        /// <summary>
        ///  Obtain a subdomain by target
        /// </summary>
        /// <param name="target">The targe</param>
        /// <param name="rootDomain">The root domain</param>
        /// <param name="subdomain">The subdomain</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A subdomain</returns>
        Task<Subdomain> GetSubdomainAsync(Target target, RootDomain rootDomain, string subdomain, CancellationToken cancellationToken = default);

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

        /// <summary>
        /// Update the agent that ran in the subdomain
        /// </summary>
        /// <param name="subdomain">The subdomain</param>
        /// <param name="name">The agentName</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        Task UpdateSubdomainAgentAsync(Subdomain subdomain, string agentName, CancellationToken cancellationToken = default);
    }
}
