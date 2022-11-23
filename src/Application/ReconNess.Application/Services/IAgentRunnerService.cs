using ReconNess.Application.Models;
using ReconNess.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Application.Services;

/// <summary>
/// The interface IAgentRunnerService
/// </summary>
public interface IAgentRunnerService : IService<AgentRunner>
{
    /// <summary>
    /// Run the agent
    /// </summary>
    /// <param name="agentRunnerInfo">The agent run parameters</param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>A Channel</returns>
    Task<string> RunAgentAsync(AgentRunnerInfo agentRunnerInfo, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stop the agent if it is running
    /// </summary>
    /// <param name="agentRunner">The agent run parameters</param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>A task</returns>
    Task StopAgentAsync(string channel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtain a list of channels that still are running
    /// </summary>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>A list of channels that still are running</returns>        
    Task<IEnumerable<string>> RunningAgentsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtain the terminal output from a channel
    /// </summary>
    /// <param name="channel">The channel</param>
    /// <param name="cancellationToken">Notification that operations should be canceled<</param>
    /// <returns>The terminal output from a channel</returns>
    Task<IEnumerable<string>> TerminalOutputAsync(string channel, CancellationToken cancellationToken = default);
}
