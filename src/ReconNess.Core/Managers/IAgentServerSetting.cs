using ReconNess.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Core.Managers
{
    /// <summary>
    /// The interface IAgentServerSetting
    /// </summary>
    public interface IAgentServerSetting
    {
        /// <summary>
        /// Obtain the agents setting
        /// </summary>
        /// <returns>The agents setting</returns>
        Task<AgentsSetting> GetAgentSettingAsync(CancellationToken cancellationToken = default);
    }
}
