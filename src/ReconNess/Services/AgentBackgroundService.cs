using Microsoft.Extensions.DependencyInjection;
using ReconNess.Core.Models;
using ReconNess.Core.Services;
using ReconNess.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Services
{
    /// <summary>
    /// This class implement <see cref="IAgentBackgroundService"/>
    /// </summary>
    public class AgentBackgroundService : IAgentBackgroundService
    {
        private readonly IServiceProvider serviceProvider;

        /// <summary>
        ///  Initializes a new instance of the <see cref="AgentBackgroundService" /> class
        /// </summary>
        /// <param name="serviceProvider"><see cref="IServiceProvider"/></param>
        public AgentBackgroundService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// <see cref="IAgentBackgroundService.SaveOutputParseOnScopeAsync(AgentRunner, ScriptOutput, CancellationToken)"/>
        /// </summary>
        public async Task SaveOutputParseOnScopeAsync(AgentRunner agentRun, ScriptOutput terminalOutputParse, CancellationToken cancellationToken = default)
        {
            using (var scope = this.serviceProvider.CreateScope())
            {
                var targetService =
                    scope.ServiceProvider
                        .GetRequiredService<ITargetService>();

                await targetService.SaveTerminalOutputParseAsync(agentRun, terminalOutputParse, cancellationToken);
            }
        }

        /// <summary>
        /// <see cref="IAgentBackgroundService.UpdateLastRunAgentOnScopeAsync(Agent, CancellationToken)"/>
        /// </summary>
        public async Task UpdateLastRunAgentOnScopeAsync(Agent agent, CancellationToken cancellationToken = default)
        {
            using (var scope = this.serviceProvider.CreateScope())
            {
                var agentService =
                    scope.ServiceProvider
                        .GetRequiredService<IAgentService>();

                agent.LastRun = DateTime.Now;
                await agentService.UpdateAsync(agent, cancellationToken);
            }
        }

        /// <summary>
        /// <see cref="IAgentBackgroundService.UpdateSubdomainAgentOnScopeAsync(AgentRunner, CancellationToken)"/>
        /// </summary>
        public async Task UpdateSubdomainAgentOnScopeAsync(AgentRunner agentRun, CancellationToken cancellationToken = default)
        {
            using (var scope = this.serviceProvider.CreateScope())
            {
                var subdomainService =
                    scope.ServiceProvider
                        .GetRequiredService<ISubdomainService>();

                await subdomainService.UpdateSubdomainAgentAsync(agentRun.Subdomain, agentRun.Agent.Name, cancellationToken);
            }
        }

        /// <summary>
        /// <see cref="IAgentBackgroundService.SendNotificationOnScopeAsync(string, CancellationToken)"/>
        /// </summary>
        public async Task SendNotificationOnScopeAsync(string payload, CancellationToken cancellationToken = default)
        {
            using (var scope = this.serviceProvider.CreateScope())
            {
                var notificationService =
                    scope.ServiceProvider
                        .GetRequiredService<INotificationService>();

                await notificationService.SendAsync(payload, cancellationToken);
            }
        }
    }
}