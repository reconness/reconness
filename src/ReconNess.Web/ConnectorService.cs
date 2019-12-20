using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using ReconNess.Core.Services;

namespace ReconNess.Web
{
    /// <summary>
    /// This class implement <see cref="IConnectorService"/>
    /// </summary>
    public class ConnectorService : IConnectorService
    {
        private readonly IHubContext<ReconnetHub> reconnetHub;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectorService" /> class
        /// </summary>
        /// <param name="reconnetHub">The hub context</param>
        public ConnectorService(IHubContext<ReconnetHub> reconnetHub)
        {
            this.reconnetHub = reconnetHub;
        }

        /// <summary>
        /// <see cref="IConnectorService.SendAsync(string, string, CancellationToken)"/>
        /// </summary>
        public async Task SendAsync(string method, string msg, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await this.reconnetHub.Clients.All.SendAsync(method, msg, cancellationToken);
        }
    }
}