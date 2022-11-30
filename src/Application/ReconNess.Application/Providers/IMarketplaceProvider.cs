using ReconNess.Application.Models;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Application.Providers;

/// <summary>
/// This interface provider define access to the markeplace 3-party
/// </summary>
public interface IMarketplaceProvider
{
    /// <summary>
    /// Obtain the list of Agents in the marketplace
    /// </summary>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>The list of Agents in the marketplace</returns>
    Task<AgentMarketplaces?> GetAgentMarketplacesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtain the agent default script
    /// </summary>
    /// <param name="scriptUrl">The url with the default script</param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>The agent default script</returns>
    Task<string> GetScriptAsync(string scriptUrl, CancellationToken cancellationToken);
}
