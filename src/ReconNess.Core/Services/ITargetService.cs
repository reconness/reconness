using ReconNess.Core.Models;
using ReconNess.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Core.Services
{
    /// <summary>
    /// The interface ILabelService
    /// </summary>
    public interface ITargetService : IService<Target>, ISaveTerminalOutputParseService<Target>
    {
        /// <summary>
        /// Obtain the list of targets not tracking
        /// </summary>
        /// <param name="predicate">The predicate</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The list of target</returns>
        Task<List<Target>> GetTargetsNotTrackingAsync(Expression<Func<Target, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtain the target not tracking
        /// </summary>
        /// <param name="predicate">The predicate</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The target</returns>
        Task<Target> GetTargetNotTrackingAsync(Expression<Func<Target, bool>> predicate, CancellationToken cancellationToken = default);

        // <summary>
        /// Obtain the target
        /// </summary>
        /// <param name="predicate">The predicate</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The target</returns>
        Task<Target> GetTargetAsync(Expression<Func<Target, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtain the target dashboard
        /// </summary>
        /// <param name="target">The target</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The target dashboard data</returns>
        Task<TargetDashboard> GetDashboardAsync(Target target, CancellationToken cancellationToken = default);
    }
}
