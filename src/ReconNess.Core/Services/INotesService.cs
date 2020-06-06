using ReconNess.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Core.Services
{
    /// <summary>
    /// The interface INotesService
    /// </summary>
    public interface INotesService : IService<Note>
    {
        /// <summary>
        /// Save target notes
        /// </summary>
        /// <param name="target">The target</param>
        /// <param name="notes">The notes</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns></returns>
        Task SaveTargetNotesAsync(RootDomain target, string notes, CancellationToken cancellationToken = default);

        /// <summary>
        /// Save subdomains notes
        /// </summary>
        /// <param name="subdomain">The subdomain</param>
        /// <param name="notes">The notes</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns></returns>
        Task SaveSubdomainNotesAsync(Subdomain subdomain, string notes, CancellationToken cancellationToken = default);
    }
}
