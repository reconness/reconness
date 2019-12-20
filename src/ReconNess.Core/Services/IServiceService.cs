using System.Threading.Tasks;
using ReconNess.Entities;

namespace ReconNess.Core.Services
{
    /// <summary>
    /// The interface IServiceService
    /// </summary>
    public interface IServiceService : IService<Service>
    {
        Task<bool> IsAssignedToSubdomainAsync(Subdomain subdomain, Service service);
    }
}
