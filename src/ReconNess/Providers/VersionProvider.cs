using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ReconNess.Core.Providers;
using RestSharp;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Providers
{
    /// <summary>
    /// This class implement <see cref="IVersionProvider"/>
    /// </summary>
    public class VersionProvider : IVersionProvider
    {
        private readonly ILogger<VersionProvider> logger;

        private readonly IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionProvider" /> class
        /// </summary>
        /// <param name="logger"><see cref="ILogger{TCategoryName}"/></param>
        /// <param name="configuration"><see cref="IConfiguration"/></param>
        public VersionProvider(ILogger<VersionProvider> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;
        }

        /// <summary>
        /// <see cref="IVersionProvider.GetLatestVersionAsync(CancellationToken)"/>
        /// </summary>
        public async Task<string> GetLatestVersionAsync(CancellationToken cancellationToken)
        {
            try
            {
                var client = new RestClient("https://version.reconness.com/");
                var request = new RestRequest();
                client.UserAgent = "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/44.0.2403.157 Safari/537.36";
                var response = await client.ExecuteGetAsync(request, cancellationToken);

                var match = Regex.Match(response.Content, @"<body>\n(.*)\n</body>");
                if (match.Success && match.Groups.Count == 2)
                {
                    return match.Groups[1].ToString();
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
            }

            return string.Empty;
        }

        /// <summary>
        /// <see cref="IVersionProvider.GetCurrentVersionAsync(CancellationToken)"/>
        /// </summary>
        public Task<string> GetCurrentVersionAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(this.configuration["ReconNess:Version"]);
        }
    }
}
