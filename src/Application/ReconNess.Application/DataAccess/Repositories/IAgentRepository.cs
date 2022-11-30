using ReconNess.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Linq.Expressions;
using System;

namespace ReconNess.Application.DataAccess.Repositories;

/// <summary>
/// This interface is a custom agent repository
/// </summary>
public interface IAgentRepository : IRepository<Agent>
{
    /// <summary>
    /// Obtain all the Agents with not tracking
    /// </summary>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>List of Agents with categories or null</returns>
    Task<List<Agent>> GetAgentsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtain an Agent with not tracking
    /// </summary>
    /// <param name="criteria"></param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>A Agent or null</returns>
    Task<Agent?> GetAgentAsync(Expression<Func<Agent, bool>> criteria, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtain an Agent
    /// </summary>
    /// <param name="criteria"></param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>A Agent or null</returns>
    Task<Agent?> GetAgentWithCategoriesTriggerAndEventsAsync(Expression<Func<Agent, bool>> criteria, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtain an Agent to Run
    /// </summary>
    /// <param name="criteria"></param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>A Agent or null</returns>
    Task<Agent?> GetAgentToRunAsync(Expression<Func<Agent, bool>> criteria, CancellationToken cancellationToken = default);
}
