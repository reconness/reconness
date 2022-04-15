using ReconNess.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Core.Services
{
    /// <summary>
    /// The interface IAgentRunningService
    /// </summary>
    public interface IAgentServerService : IService<AgentServer>
    {
        /// <summary>
        /// Obtain the available server to run the command
        /// </summary>
        /// <returns>The available server to run the command</returns>
        Task<int> GetAgentAvailableServerAsync(CancellationToken cancellationToken = default);
    }
}
