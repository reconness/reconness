
using NLog;
using ReconNess.Application.Models;
using ReconNess.Application.Providers;
using RestSharp;
using System.Text.Json;

namespace ReconNess.Infrastructure.Providers;

/// <summary>
/// This class implement the provider interface <see cref="IMarketplaceProvider"/>
/// </summary>
public class MarketplaceProvider : IMarketplaceProvider
{
    protected static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    /// <inheritdoc/>
    public async Task<AgentMarketplaces?> GetAgentMarketplacesAsync(CancellationToken cancellationToken = default)
    {
        var client = new RestClient("https://raw.githubusercontent.com/");
        var request = new RestRequest("/reconness/reconness-agents/master/default-agents2.json");

        var response = await client.ExecuteGetAsync(request, cancellationToken);
        if (response == null || string.IsNullOrEmpty(response.Content))
        {
            return null;
        }

        var agentMarketplaces = JsonSerializer.Deserialize<AgentMarketplaces>(response.Content);

        return agentMarketplaces;
    }

    /// <inheritdoc/>
    public async Task<string> GetScriptAsync(string scriptUrl, CancellationToken cancellationToken)
    {
        try
        {
            var client = new RestClient(scriptUrl);
            var request = new RestRequest();

            var response = await client.ExecuteGetAsync(request, cancellationToken);

            return response.Content ?? string.Empty;
        }
        catch (Exception ex)
        {
            _logger.Error(ex);
        }

        return string.Empty;
    }
}
