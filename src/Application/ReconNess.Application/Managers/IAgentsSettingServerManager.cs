using ReconNess.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Application.Managers;

/// <summary>
/// The interface IAgentServerSetting
/// </summary>
public interface IAgentsSettingServerManager
{
    /// <summary>
    /// Obtain the agents setting
    /// </summary>
    /// <returns>The agents setting</returns>
    Task<AgentsSetting?> GetAgentSettingAsync(CancellationToken cancellationToken = default);
}
