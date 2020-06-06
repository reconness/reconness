using Microsoft.AspNetCore.SignalR;
using ReconNess.Core.Services;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Web
{
    /// <summary>
    /// This class implement <see cref="IConnectorService"/>
    /// </summary>
    public class ConnectorService : IConnectorService
    {
        private readonly IHubContext<ReconNessHub> reconnessHub;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectorService" /> class
        /// </summary>
        /// <param name="reconnessHub">The hub context</param>
        public ConnectorService(IHubContext<ReconNessHub> reconnessHub)
        {
            this.reconnessHub = reconnessHub;
        }

        /// <summary>
        /// <see cref="IConnectorService.SendAsync(string, string, CancellationToken)"/>
        /// </summary>
        public async Task SendAsync(string method, string msg, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await this.reconnessHub.Clients.All.SendAsync(method, msg, cancellationToken);
        }
    }
}