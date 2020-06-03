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
    /// The interface ITargetService
    /// </summary>
    public interface IRootDomainService : IService<RootDomain>
    {
        /// <summary>
        /// Obtain a target with subdomains
        /// </summary>
        /// <param name="criteria">The criteria</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A target with subdomains</returns>
        Task<RootDomain> GetDomainWithSubdomainsAsync(Expression<Func<RootDomain, bool>> criteria, CancellationToken cancellationToken = default);

        /// <summary>
        /// Save the output that the ScriptEnginer returned on database
        /// </summary>
        /// <param name="domain">The target domain</param>
        /// <param name="subdomain">the subdomain.async Can be null is the agent run only for the target</param>
        /// <param name="agent">The agent was ran</param>
        /// <param name="scriptOutput">The output that the ScriptEnginer returned</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A Task</returns>
        Task SaveScriptOutputAsync(RootDomain domain, Subdomain subdomain, Agent agent, ScriptOutput scriptOutput, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete the target with all the subdomains and relations
        /// </summary>
        /// <param name="domain">The target to delete</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        Task DeleteDomainAsync(RootDomain domain, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete all the subdomains and relations
        /// </summary>
        /// <param name="domain">Target to delete all the subdomains</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        Task DeleteSubdomainsAsync(RootDomain domain, CancellationToken cancellationToken = default);

        /// <summary>
        /// Upload all the subdomains
        /// </summary>
        /// <param name="domain">Target to upload all the subdomains</param>
        /// <param name="uploadSubdomains">Subdomains to upload</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        Task<List<Subdomain>> UploadSubdomainsAsync(RootDomain domain, IEnumerable<string> uploadSubdomains, CancellationToken cancellationToken = default);
    }
}
