using Microsoft.Extensions.Configuration;
using NLog;
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

        /// <summary>	
        /// <see cref="IVersionProvider.GetLatestVersionAsync(CancellationToken)"/>	
        /// </summary>	
        public async Task<string> GetLatestVersionAsync(CancellationToken cancellationToken)
        {
            try
            {
                var client = new RestClient("https://version.reconness.com/");
                var request = new RestRequest();
                var response = await client.ExecuteGetAsync(request, cancellationToken);

                var match = Regex.Match(response.Content, @"<body>\n(.*)\n</body>");
                if (match.Success && match.Groups.Count == 2)
                {
                    return match.Groups[1].ToString();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
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
