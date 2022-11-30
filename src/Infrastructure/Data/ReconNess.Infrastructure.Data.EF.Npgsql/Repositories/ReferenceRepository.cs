using Microsoft.EntityFrameworkCore;
using ReconNess.Application.DataAccess;
using ReconNess.Application.DataAccess.Repositories;
using ReconNess.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Infrastructure.Data.EF.Npgsql.Repositories;

/// <summary>
/// This class implement <see cref="IReferenceRepository"/>
/// </summary>
internal class ReferenceRepository : Repository<Reference>, IReferenceRepository, IRepository<Reference>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReferenceRepository" /> class
    /// </summary>
    /// <param name="context">The implementation of Database Context <see cref="IDbContext" /></param>
    public ReferenceRepository(IDbContext context) : base(context)
    {
    }

    /// <inheritdoc/>
    public async Task<List<Reference>> GetReferencesOrderByCategoriesAsync(CancellationToken cancellationToken) => await GetAllQueryable()
                .OrderBy(r => r.Categories)
                .AsNoTracking()
            .ToListAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task<List<string>> GetAllCategoriesAsync(CancellationToken cancellationToken) => await GetAllQueryable()
                .Select(r => r.Categories)
                .AsNoTracking()
            .ToListAsync(cancellationToken);
}
