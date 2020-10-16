using ReconNess.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
        Task<List<Subdomain>> GetAllWithIncludesAsync(Target target, RootDomain rootDomain, string subdomain, CancellationToken cancellationToken = default);

        /// <summary>
        ///  Obtain a subdomain by target and root domain
        /// </summary>
        /// <param name="target">The targe</param>
        /// <param name="rootDomain">The root domain</param>
        /// <param name="subdomain">The subdomain</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A subdomain</returns>
        Task<Subdomain> GetWithIncludeAsync(Target target, RootDomain rootDomain, string subdomain, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtain a subdomain by criteria
        /// </summary>
        /// <param name="predicate">The predicate</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns></returns>
        Task<Subdomain> GetWithIncludeAsync(Expression<Func<Subdomain, bool>> predicate, CancellationToken cancellationToken = default);
    }
}
