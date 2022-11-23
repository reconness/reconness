using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Application.Managers;

/// <summary>
/// The interface IAgentRunningService
/// </summary>
public interface IAgentServerManager
{
    /// <summary>
    /// Obtain the available server to run the command
    /// </summary>
    /// <param name="channel"></param>
    /// <param name="refreshInMin"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The available server to run the command</returns>
    Task<int> GetAvailableServerAsync(string channel, int refreshInMin = 60, CancellationToken cancellationToken = default);
}