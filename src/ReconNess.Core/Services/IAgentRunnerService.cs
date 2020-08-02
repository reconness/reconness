using ReconNess.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Core.Services
{
    /// <summary>
    /// The interface IAgentRunnerService
    /// </summary>
    public interface IAgentRunnerService : IService<Agent>
    {
        /// <summary>
        /// Run the agent
        /// </summary>
        /// <param name="target">The target</param>
        /// <param name="rootDomain">The root domain</param>
        /// <param name="subdomain">The subdomain</param>
        /// <param name="agent">The agent</param>
        /// <param name="command">The command to run</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        Task RunAsync(Target target, RootDomain rootDomain, Subdomain subdomain, Agent agent, string command, bool activateNotification, CancellationToken cancellationToken = default);

        /// <summary>
        /// Stop the agent if it is running
        /// </summary>
        /// <param name="target">The target</param>
        /// <param name="rootDomain">The root domain</param>
        /// <param name="subdomain">The subdomain</param>
        /// <param name="agent">The agent</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        Task StopAsync(Target target, RootDomain rootDomain, Subdomain subdomain, Agent agent, CancellationToken cancellationToken = default);
    }
}
