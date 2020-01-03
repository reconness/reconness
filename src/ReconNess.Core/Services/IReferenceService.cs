using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ReconNess.Entities;

namespace ReconNess.Core.Services
{
    /// <summary>
    /// The interface IReferenceService
    /// </summary>
    public interface IReferenceService : IService<Reference>
    {
        /// <summary>
        /// Obtain all the reference categories saved before
        /// </summary>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The list of all the reference categories saved before</returns>
        Task<List<string>> GetAllCategoriesAsync(CancellationToken cancellationToken);
    }
}
