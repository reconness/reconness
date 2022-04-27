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
    /// The interface ISubdomainService
    /// </summary>
    public interface ISubdomainService : IService<Subdomain>, ISaveTerminalOutputParseService<Subdomain>
    {
        /// <summary>
        /// Obtain the list of subdomains
        /// </summary>
        /// <param name="predicate">The predicate</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The list of subdomains</returns>
        Task<List<Subdomain>> GetSubdomainsNoTrackingAsync(Expression<Func<Subdomain, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtain a subdomain by criteria
        /// </summary>
        /// <param name="predicate">The predicate</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns></returns>
        Task<Subdomain> GetSubdomainAsync(Expression<Func<Subdomain, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtain a subdomain by criteria no tracking
        /// </summary>
        /// <param name="predicate">The predicate</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns></returns>
        Task<Subdomain> GetSubdomainNoTrackingAsync(Expression<Func<Subdomain, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtain the Subdomains paged and filtered
        /// </summary>
        /// <param name="rootDomain">The rootdomain</param>
        /// <param name="query">The query to filter</param>
        /// <param name="page">The page</param>
        /// <param name="limit">The limit</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The Subdomains paged and filtered</returns>
        Task<PagedResult<Subdomain>> GetPaginateAsync(RootDomain rootDomain, string query, int page, int limit, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtain a subdomain by criteria
        /// </summary>
        /// <param name="predicate">The predicate</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns></returns>
        Task<Subdomain> GetWithLabelsAsync(Expression<Func<Subdomain, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Add a new Label to the subdomain
        /// </summary>
        /// <param name="subdomain">The subdomian</param>
        /// <param name="newLabel">The new label to add</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A Task</returns>
        Task AddLabelAsync(Subdomain subdomain, string newLabel, CancellationToken cancellationToken = default);
    }
}
