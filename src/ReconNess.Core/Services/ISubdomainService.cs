using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ReconNess.Core.Models;
using ReconNess.Entities;

namespace ReconNess.Core.Services
{
    /// <summary>
    /// The interface ISubdomainService
    /// </summary>
    public interface ISubdomainService : IService<Subdomain>
    {
        /// <summary>
        /// Update the subdomain with the output for the terminal
        /// Add IpAddress, if is alive, if HasHttpOpen and Services running on the subdomain
        /// </summary>
        /// <param name="subdomain">The subdomain</param>
        /// <param name="agent">The agent</param>
        /// <param name="scriptOutput">The terminal output one line</param>
        /// <param name="newSubdomain">If is new subdomain</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        Task UpdateSubdomainAsync(Subdomain subdomain, Agent agent, ScriptOutput scriptOutput, bool newSubdomain, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete all the subdomains with the services
        /// </summary>
        /// <param name="subdomains">The list subdomains to delete</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        void DeleteSubdomains(ICollection<Subdomain> subdomains, CancellationToken cancellationToken = default);
    }
}
