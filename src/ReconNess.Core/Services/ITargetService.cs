using ReconNess.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Core.Services
{
    /// <summary>
    /// The interface ILabelService
    /// </summary>
    public interface ITargetService : IService<Target>, ISaveTerminalOutputParseService<Target>
    {
        /// <summary>
        /// Obtain the list of target with includes
        /// </summary>
        /// <param name="predicate">The predicate</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The list of target with includes</returns>
        Task<List<Target>> GetAllWithRootDomainsAsync(Expression<Func<Target, bool>> predicate, CancellationToken cancellationToken);

        /// <summary>
        /// Obtain the target with the include references
        /// </summary>
        /// <param name="predicate">The predicate</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The target with the include references</returns>
        Task<Target> GetWithRootDomainAsync(Expression<Func<Target, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Upload root domain data with subdomains, services, port, ips, directories, labels, etc
        /// </summary>
        /// <param name="target">Current target</param>
        /// <param name="newRootdomain">root domain upload</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        Task UploadRootDomainAsync(Target target, RootDomain newRootdomain, CancellationToken cancellationToken = default);
    }
}
