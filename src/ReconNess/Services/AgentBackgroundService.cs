using Microsoft.Extensions.DependencyInjection;
using NLog;
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
        protected static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

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
        /// <see cref="IAgentBackgroundService.SaveOutputParseOnScopeAsync(AgentRunner, string, ScriptOutput, CancellationToken)"/>
        /// </summary>
        public async Task SaveOutputParseOnScopeAsync(AgentRunner agentRun, string agentRunType, ScriptOutput terminalOutputParse, CancellationToken cancellationToken = default)
        {
            using (var scope = this.serviceProvider.CreateScope())
            {
                if (AgentRunnerTypes.ALL_TARGETS.Equals(agentRunType, StringComparison.CurrentCultureIgnoreCase) ||
                    AgentRunnerTypes.CURRENT_TARGET.Equals(agentRunType, StringComparison.CurrentCultureIgnoreCase))
                {
                    var targetService = scope.ServiceProvider.GetRequiredService<ITargetService>();
                    await targetService.SaveTerminalOutputParseAsync(agentRun.Target, agentRun.ActivateNotification, terminalOutputParse, cancellationToken);
                }
                else if (AgentRunnerTypes.ALL_ROOTDOMAINS.Equals(agentRunType, StringComparison.CurrentCultureIgnoreCase) ||
                        AgentRunnerTypes.CURRENT_ROOTDOMAIN.Equals(agentRunType, StringComparison.CurrentCultureIgnoreCase))
                {
                    var rootDomainService = scope.ServiceProvider.GetRequiredService<IRootDomainService>();
                    await rootDomainService.SaveTerminalOutputParseAsync(agentRun.RootDomain, agentRun.ActivateNotification, terminalOutputParse, cancellationToken);
                }
                else if (AgentRunnerTypes.ALL_SUBDOMAINS.Equals(agentRunType, StringComparison.CurrentCultureIgnoreCase) ||
                        AgentRunnerTypes.CURRENT_SUBDOMAIN.Equals(agentRunType, StringComparison.CurrentCultureIgnoreCase))
                {
                    var subdomainService = scope.ServiceProvider.GetRequiredService<ISubdomainService>();
                    await subdomainService.SaveTerminalOutputParseAsync(agentRun.Subdomain, agentRun.ActivateNotification, terminalOutputParse, cancellationToken);
                }
            }
        }

        /// <summary>
        /// <see cref="IAgentBackgroundService.UpdateSubdomainAgentOnScopeAsync(AgentRunner, string, CancellationToken)"/>
        /// </summary>
        public async Task UpdateAgentOnScopeAsync(AgentRunner agentRun, string agentRunType, CancellationToken cancellationToken = default)
        {
            using (var scope = this.serviceProvider.CreateScope())
            {
                if (AgentRunnerTypes.ALL_TARGETS.Equals(agentRunType, StringComparison.CurrentCultureIgnoreCase) ||
                    AgentRunnerTypes.CURRENT_TARGET.Equals(agentRunType, StringComparison.CurrentCultureIgnoreCase))
                {
                    var targetService = scope.ServiceProvider.GetRequiredService<ITargetService>();
                    await targetService.UpdateAgentRanAsync(agentRun.Target, agentRun.Agent.Name, cancellationToken);
                }
                else if (AgentRunnerTypes.ALL_ROOTDOMAINS.Equals(agentRunType, StringComparison.CurrentCultureIgnoreCase) ||
                        AgentRunnerTypes.CURRENT_ROOTDOMAIN.Equals(agentRunType, StringComparison.CurrentCultureIgnoreCase))
                {
                    var rootDomainService = scope.ServiceProvider.GetRequiredService<IRootDomainService>();
                    await rootDomainService.UpdateAgentRanAsync(agentRun.RootDomain, agentRun.Agent.Name, cancellationToken);
                }
                else if (AgentRunnerTypes.ALL_SUBDOMAINS.Equals(agentRunType, StringComparison.CurrentCultureIgnoreCase) ||
                        AgentRunnerTypes.CURRENT_SUBDOMAIN.Equals(agentRunType, StringComparison.CurrentCultureIgnoreCase))
                {
                    var subdomainService = scope.ServiceProvider.GetRequiredService<ISubdomainService>();
                    await subdomainService.UpdateAgentRanAsync(agentRun.Subdomain, agentRun.Agent.Name, cancellationToken);
                }
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