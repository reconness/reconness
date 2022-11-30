using Microsoft.EntityFrameworkCore;
using ReconNess.Application.DataAccess;
using ReconNess.Application.DataAccess.Repositories;
using ReconNess.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Infrastructure.Data.EF.Npgsql.Repositories;

/// <summary>
/// This class implement <see cref="IDirectoryRepository"/>
/// </summary>
internal class DirectoryRepository : Repository<Directory>, IDirectoryRepository, IRepository<Directory>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DirectoryRepository" /> class
    /// </summary>
    /// <param name="context">The implementation of Database Context <see cref="IDbContext" /></param>
    public DirectoryRepository(IDbContext context) : base(context)
    {
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Directory>> GetDirectoriesWithSubdoaminsAsync(Expression<Func<Directory, bool>> criteria, CancellationToken cancellationToken = default) =>
        await GetAllQueryableByCriteria(criteria)
                .Include(d => d.Subdomain)
            .ToListAsync(cancellationToken);
}
