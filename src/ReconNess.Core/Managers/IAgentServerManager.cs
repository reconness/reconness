using ReconNess.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Core.Managers
{
    /// <summary>
    /// The interface IAgentRunningService
    /// </summary>
    public interface IAgentServerManager
    {
        /// <summary>
        /// Obtain the available server to run the command
        /// </summary>
        /// <returns>The available server to run the command</returns>
        Task<int> GetAvailableServerAsync(string channel, CancellationToken cancellationToken = default);

        /// <summary>
        /// If the Agents Setting was updated, and we change the amount of available servers, we need to call this method to invalidate the servers data struct
        /// </summary>
        void InvalidateServers();
    }
}
