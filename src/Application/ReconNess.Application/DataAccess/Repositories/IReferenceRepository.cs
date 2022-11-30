using ReconNess.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace ReconNess.Application.DataAccess.Repositories;

/// <summary>
/// This interface is a custom reference repository
/// </summary>
public interface IReferenceRepository : IRepository<Reference>
{
    /// <summary>
    /// Obtain the list of references order by categories
    /// </summary>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>The list of references</returns>
    Task<List<Reference>> GetReferencesOrderByCategoriesAsync(CancellationToken cancellationToken);

    //// <summary>
    /// Obtain all the reference categories
    /// </summary>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>The list of all the reference categories</returns>
    Task<List<string>> GetAllCategoriesAsync(CancellationToken cancellationToken);
}
