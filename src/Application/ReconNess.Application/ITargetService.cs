using ReconNess.Application.Models;
using ReconNess.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Application;

/// <summary>
/// The interface ILabelService
/// </summary>
public interface ITargetService : IService<Target>
{
    /// <summary>
    /// Obtain the list of targets not tracking
    /// </summary>
    /// <param name="predicate">The predicate</param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>The list of target</returns>
    Task<List<Target>> GetTargetsAsync(Expression<Func<Target, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtain the target not tracking
    /// </summary>
    /// <param name="predicate">The predicate</param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>The target</returns>
    Task<Target?> GetTargetAsync(Expression<Func<Target, bool>> predicate, CancellationToken cancellationToken = default);

    // <summary>
    /// Obtain the target with rootdomains
    /// </summary>
    /// <param name="predicate">The predicate</param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>The target</returns>
    Task<Target?> GetTargetWithRootdomainsAsync(Expression<Func<Target, bool>> predicate, CancellationToken cancellationToken = default);

    // <summary>
    /// Obtain the target with notes
    /// </summary>
    /// <param name="predicate">The predicate</param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>The target</returns>
    Task<Target?> GetTargetWithNotesAsync(Expression<Func<Target, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Export the target
    /// </summary>
    /// <param name="predicate">The predicate</param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns></returns>
    Task<Target?> ExportTargetAsync(Expression<Func<Target, bool>> criteria, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtain the target dashboard
    /// </summary>
    /// <param name="target">The target</param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>The target dashboard data</returns>
    Task<TargetDashboard> GetDashboardAsync(string targetName, CancellationToken cancellationToken = default);

}
