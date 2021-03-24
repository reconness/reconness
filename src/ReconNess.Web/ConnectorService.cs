using Microsoft.AspNetCore.SignalR;
using ReconNess.Core.Services;
using System;
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

        /// <inheritdoc/>
        public async Task SendAsync(string channel, string msg, bool includeTime = true, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var time = DateTime.Now.ToString("hh:mm:ss tt");

            msg = includeTime ? $"[{time}] {msg}" : msg;
            await this.reconnessHub.Clients.All.SendAsync(channel, msg, cancellationToken);
        }
    }
}