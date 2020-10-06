using Microsoft.Extensions.Configuration;
using ReconNess.Core.Providers;
using RestSharp;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Providers
{
    /// <summary>
    /// 
    /// </summary>
    public class VersionProvider : IVersionProvider
    {
        private readonly IConfiguration configuration;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public VersionProvider(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<string> GetLatestVersionAsync(CancellationToken cancellationToken)
        {
            var currentVersion = this.configuration["ReconNess:Version"];
            if (string.IsNullOrEmpty(currentVersion))
            {
                return string.Empty;
            }

            try
            {
                var client = new RestClient("https://version.reconness.com/");
                var request = new RestRequest();

                var response = await client.ExecuteGetAsync(request, cancellationToken);

                var match = System.Text.RegularExpressions.Regex.Match(response.Content, @"<body>\n(.*)\n</body>");
                if (match.Success && match.Groups.Count == 2)
                {
                    var latestVersion = match.Groups[1].ToString();
                    int comparison = string.Compare(currentVersion, latestVersion, comparisonType: StringComparison.OrdinalIgnoreCase);
                    if (comparison < 0)
                    {
                        return $"[Latest v{latestVersion}]";
                    }
                }
            }
            catch
            {

            }

            return string.Empty;
        }
    }
}
