using ReconNess.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Linq.Expressions;
using System;

namespace ReconNess.Application.DataAccess.Repositories;

/// <summary>
/// This interface is a custom event track repository
/// </summary>
public interface IDirectoryRepository : IRepository<Directory>
{
    /// <summary>
    /// Obtain a list of Event Tracks
    /// </summary>
    /// <param name="criteria">The criteria</param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>A list of Event Tracks</returns>
    Task<IEnumerable<Directory>> GetDirectoriesWithSubdoaminsAsync(Expression<Func<Directory, bool>> criteria, CancellationToken cancellationToken = default);
}
