using ReconNess.Domain.Entities;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Collections.Generic;

namespace ReconNess.Application.DataAccess.Repositories;

/// <summary>
/// This interface is a custom rootdomain repository
/// </summary>
public interface IRootDomainRepository : IRepository<RootDomain>
{
    /// <summary>
    /// Obtain a rootDomain with no tracking
    /// </summary>
    /// <param name="criteria">The criteria</param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>A subdomain or null</returns>
    Task<RootDomain?> GetRootDomainAsync(Expression<Func<RootDomain, bool>> criteria, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtain a rootDomain with subdomains only
    /// </summary>
    /// <param name="criteria">The criteria</param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>A subdomain or null</returns>
    Task<RootDomain?> GetRootDomainWithSubdomainsAsync(Expression<Func<RootDomain, bool>> criteria, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtain a list of rootDomains with targets
    /// </summary>
    /// <param name="criteria">The criteria</param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>A subdomain or null</returns>
    Task<IEnumerable<RootDomain>> GetRootDomainsWithTargetsAsync(Expression<Func<RootDomain, bool>> criteria, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtain a rootDomain with notes only
    /// </summary>
    /// <param name="criteria">The criteria</param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>A subdomain or null</returns>
    Task<RootDomain?> GetRootDomainWithNotesAsync(Expression<Func<RootDomain, bool>> criteria, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtain a rootDomain with subdomains
    /// </summary>
    /// <param name="criteria">The criteria</param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>A subdomain or null</returns>
    Task<RootDomain?> ExportRootDomainAsync(Expression<Func<RootDomain, bool>> criteria, CancellationToken cancellationToken = default);
}
