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
    public interface IRootDomainService : IService<RootDomain>, ISaveTerminalOutputParseService<RootDomain>
    {
        /// <summary>
        /// Obtain a rootDomain with subdomains
        /// </summary>
        /// <param name="criteria">The criteria</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        Task<RootDomain> GetWithSubdomainsAsync(Expression<Func<RootDomain, bool>> criteria, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete all the subdomains and relations
        /// </summary>
        /// <param name="rootDomain">Target to delete all the subdomains</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        Task DeleteSubdomainsAsync(RootDomain rootDomain, CancellationToken cancellationToken = default);

        /// <summary>
        /// Upload all the subdomains
        /// </summary>
        /// <param name="rootDomain">Target to upload all the subdomains</param>
        /// <param name="uploadSubdomains">Subdomains to upload</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        Task UploadSubdomainsAsync(RootDomain rootDomain, IEnumerable<string> uploadSubdomains, CancellationToken cancellationToken = default);

        /// <summary>
        ///  Obtain the list of rootdomains from database, if does not exist create the rootdomain
        /// </summary>
        /// <param name="rootDomains"></param>
        /// <param name="lists"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>A list of subdomain added</returns>
        ICollection<RootDomain> GetRootDomains(ICollection<RootDomain> myRootDomains, List<string> newRootDomains, CancellationToken cancellationToken = default);

        /// <summary>
        /// Upload root domain data with subdomains, services, port, ips, directories, labels, etc
        /// </summary>
        /// <param name="rootDomain">Current root domain</param>
        /// <param name="uploadRootDomain">root domain upload</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        Task UploadRootDomainAsync(RootDomain rootDomain, RootDomain uploadRootDomain, CancellationToken cancellationToken = default);
    }
}
