using ReconNess.Application.DataAccess;
using ReconNess.Application.DataAccess.Repositories;
using ReconNess.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Application.Services;

/// <summary>
/// This class implement <see cref="IReferenceService"/>
/// </summary>
public class ReferenceService : Service<Reference>, IReferenceService, IService<Reference>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IReferenceService" /> class
    /// </summary>
    /// <param name="unitOfWork"><see cref="IUnitOfWork"/></param>
    public ReferenceService(IUnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    /// <inheritdoc/>
    public async Task<List<Reference>> GetReferencesAsync(CancellationToken cancellationToken)
    {
        var references = await UnitOfWork.Repository<IReferenceRepository, Reference>()
            .GetReferencesOrderByCategoriesAsync(cancellationToken);

        return references;
    }

    /// <inheritdoc/>
    public async Task<List<string>> GetAllCategoriesAsync(CancellationToken cancellationToken)
    {
        var entities = await UnitOfWork.Repository<IReferenceRepository, Reference>()
            .GetAllCategoriesAsync(cancellationToken);

        var categories = new List<string>();
        entities.ForEach(c => c.Split(',')
            .ToList()
            .ForEach(category =>
            {
                if (!categories.Contains(category))
                {
                    categories.Add(category);
                }
            }
        ));

        return categories;
    }
}
