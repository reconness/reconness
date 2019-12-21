using System.Threading;
using System.Threading.Tasks;
using ReconNess.Core;
using ReconNess.Core.Services;
using ReconNess.Entities;

namespace ReconNess.Services
{
    /// <summary>
    /// This class implement <see cref="IServiceService"/>
    /// </summary>
    public class ServiceService : Service<Service>, IService<Service>, IServiceService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserService" /> class
        /// </summary>
        /// <param name="unitOfWork"><see cref="IUnitOfWork"/></param>
        public ServiceService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        /// <summary>
        /// <see cref="IServiceService.IsAssignedToSubdomainAsync(Subdomain, Service, CancellationToken)"/>
        /// </summary>
        public async Task<bool> IsAssignedToSubdomainAsync(Subdomain subdomain, Service service, CancellationToken cancellationToken = default)
        {
            return await this.AnyAsync(s => s.Subdomain.Name == subdomain.Name && s.Name == service.Name && s.Port == service.Port, cancellationToken);
        }
    }
}
