using ReconNess.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Linq.Expressions;
using System;

namespace ReconNess.Application.DataAccess.Repositories;

/// <summary>
/// This interface is a custom agent runner repository
/// </summary>
public interface IAgentRunnerRepository : IRepository<AgentRunner>
{
    /// <summary>
    /// Obtain the agent runner using the channel
    /// </summary>
    /// <param name="channel">The channel</param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>The agent runner using the channel</returns>
    Task<AgentRunner?> GetAgentRunnerAsync(string channel, CancellationToken cancellationToken = default);


    /// <summary>
    /// Obtain a list of channels that still are running
    /// </summary>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>A list of channels that still are running</returns>  
    Task<IEnumerable<string>> RunningAgentsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtain the count base on the criteria
    /// </summary>
    /// <param name="predicate">The criteria</param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>The count base on the criteria</returns>
    Task<int> GetChannelCountAsync(Expression<Func<AgentRunner, bool>> predicate, CancellationToken cancellationToken = default);
}
