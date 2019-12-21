using System.Threading;
using System.Threading.Tasks;
using ReconNess.Entities;

namespace ReconNess.Core.Services
{
    /// <summary>
    /// The interface IServiceService
    /// </summary>
    public interface IServiceService : IService<Service>
    {
        /// <summary>
        /// Obtain if the subdomain has that services assined
        /// </summary>
        /// <param name="subdomain">The subdomain</param>
        /// <param name="service">The service</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>If the subdomain has that services assined</returns>
        Task<bool> IsAssignedToSubdomainAsync(Subdomain subdomain, Service service, CancellationToken cancellationToken = default);
    }
}
