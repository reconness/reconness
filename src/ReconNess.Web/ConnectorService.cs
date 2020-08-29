using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using ReconNess.Core.Models;
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

        /// <summary>
        /// <see cref="IConnectorService.SendAsync(string, string, CancellationToken)"/>
        /// </summary>
        public async Task SendAsync(string method, string msg, CancellationToken cancellationToken = default, bool includeTime = true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var time = DateTime.Now.ToString("hh:mm:ss tt");

            msg = includeTime ? $"[{time}] {msg}" : msg;
            await this.reconnessHub.Clients.All.SendAsync(method, msg, cancellationToken);
        }

        /// <summary>
        /// <see cref="IConnectorService.SendLogsAsync(string, string, CancellationToken)"/>
        /// </summary>
        public async Task SendLogsAsync(string channel, string msg, CancellationToken cancellationToken = default)
        {
            await this.SendAsync("logs_" + channel, msg, cancellationToken);
        }

        /// <summary>
        /// <see cref="IConnectorService.SendLogsHeadAsync(string, int, string, ScriptOutput, CancellationToken)"/>
        /// </summary>
        public async Task SendLogsHeadAsync(string channel, int lineCount, string terminalLineOutput, ScriptOutput terminalOutputParse, CancellationToken cancellationToken = default)
        {
            await this.SendLogsAsync(channel, $"Output #: {lineCount}", cancellationToken);
            await this.SendLogsAsync(channel, $"Output: {terminalLineOutput}", cancellationToken);
            await this.SendLogsAsync(channel, $"Result: {JsonConvert.SerializeObject(terminalOutputParse)}", cancellationToken);
        }

        /// <summary>
        /// <see cref="IConnectorService.SendLogsTailAsync(string, int, CancellationToken)"/>
        /// </summary>
        public async Task SendLogsTailAsync(string channel, int lineCount, CancellationToken cancellationToken = default)
        {
            await this.SendLogsAsync(channel, $"Output #: {lineCount} processed", cancellationToken);
            await this.SendLogsAsync(channel, "-----------------------------------------------------", cancellationToken);
        }
    }
}