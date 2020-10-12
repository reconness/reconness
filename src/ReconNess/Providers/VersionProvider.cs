using Microsoft.Extensions.Configuration;
using NLog;
using ReconNess.Core.Providers;
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
        /// <see cref="IVersionProvider.GetCurrentVersionAsync(CancellationToken)"/>
        /// </summary>
        public Task<string> GetCurrentVersionAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(this.configuration["ReconNess:Version"]);
        }
    }
}
