using Microsoft.Extensions.Configuration;
using NLog;
using ReconNess.Application.Providers;
using RestSharp;
using System.Text.RegularExpressions;

namespace ReconNess.Infrastructure.Providers;

/// <summary>
/// This class implement <see cref="IVersionProvider"/>
/// </summary>
public class VersionProvider : IVersionProvider
{
    protected static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    private readonly IConfiguration configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="VersionProvider" /> class
    /// </summary>
    /// <param name="logger"><see cref="ILogger{TCategoryName}"/></param>
    /// <param name="configuration"><see cref="IConfiguration"/></param>
    public VersionProvider(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    /// <inheritdoc/>
    public async Task<string> GetLatestVersionAsync(CancellationToken cancellationToken)
    {
        try
        {
            var client = new RestClient("https://version.reconness.com/");
            var request = new RestRequest();
            var response = await client.ExecuteGetAsync(request, cancellationToken);

            if (response != null && !string.IsNullOrEmpty(response.Content))
            {
                var match = Regex.Match(response.Content, @"<body>\n(.*)\n</body>");
                if (match.Success && match.Groups.Count == 2)
                {
                    return match.Groups[1].ToString();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, ex.Message);
        }

        return string.Empty;
    }

    /// <inheritdoc/>
    public string? GetCurrentVersion()
    {
        return configuration["ReconNess:Version"];
    }
}
