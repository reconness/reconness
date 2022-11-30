using ReconNess.Domain.Entities;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace ReconNess.Application.DataAccess.Repositories;

/// <summary>
/// This interface is a custom target repository
/// </summary>
public interface ITargetRepository : IRepository<Target>
{
    /// <summary>
    /// Obtain the list of targets not tracking
    /// </summary>
    /// <param name="predicate">The predicate</param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>The list of target</returns>
    Task<List<Target>> GetTargetsAsync(Expression<Func<Target, bool>> predicate, CancellationToken cancellationToken);

    /// <summary>
    /// Obtain the target not tracking
    /// </summary>
    /// <param name="predicate">The predicate</param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>The target</returns>
    Task<Target?> GetTargetAsync(Expression<Func<Target, bool>> predicate, CancellationToken cancellationToken);

    // <summary>
    /// Obtain the target
    /// </summary>
    /// <param name="predicate">The predicate</param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>The target</returns>
    Task<Target?> GetTargetWithRootdomainsAsync(Expression<Func<Target, bool>> predicate, CancellationToken cancellationToken);

    // <summary>
    /// Obtain the target with Notes
    /// </summary>
    /// <param name="predicate">The predicate</param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>The target</returns>
    Task<Target?> GetTargetWithNotesAsync(Expression<Func<Target, bool>> predicate, CancellationToken cancellationToken);

    /// <summary>
    /// Export the target
    /// </summary>
    /// <param name="predicate">The predicate</param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns></returns>
    Task<Target?> ExportTargetAsync(Expression<Func<Target, bool>> criteria, CancellationToken cancellationToken = default);
}
