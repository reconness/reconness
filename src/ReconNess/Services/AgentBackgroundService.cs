using Microsoft.Extensions.DependencyInjection;
using NLog;
using ReconNess.Core.Models;
using ReconNess.Core.Services;
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

        /// <inheritdoc/>
        public async Task SaveOutputParseOnScopeAsync(AgentRunner agentRun, string agentRunType, ScriptOutput terminalOutputParse, CancellationToken cancellationToken = default)
        {
            using var scope = this.serviceProvider.CreateScope();

            if (AgentRunnerTypes.ALL_TARGETS.Equals(agentRunType, StringComparison.CurrentCultureIgnoreCase) ||
                AgentRunnerTypes.CURRENT_TARGET.Equals(agentRunType, StringComparison.CurrentCultureIgnoreCase))
            {
                var targetService = scope.ServiceProvider.GetRequiredService<ITargetService>();
                await targetService.SaveTerminalOutputParseAsync(agentRun.Target, agentRun.Agent.Name, agentRun.ActivateNotification, terminalOutputParse, cancellationToken);
            }
            else if (AgentRunnerTypes.ALL_ROOTDOMAINS.Equals(agentRunType, StringComparison.CurrentCultureIgnoreCase) ||
                    AgentRunnerTypes.CURRENT_ROOTDOMAIN.Equals(agentRunType, StringComparison.CurrentCultureIgnoreCase))
            {
                var rootDomainService = scope.ServiceProvider.GetRequiredService<IRootDomainService>();
                await rootDomainService.SaveTerminalOutputParseAsync(agentRun.RootDomain, agentRun.Agent.Name, agentRun.ActivateNotification, terminalOutputParse, cancellationToken);
            }
            else if (AgentRunnerTypes.ALL_SUBDOMAINS.Equals(agentRunType, StringComparison.CurrentCultureIgnoreCase) ||
                    AgentRunnerTypes.CURRENT_SUBDOMAIN.Equals(agentRunType, StringComparison.CurrentCultureIgnoreCase))
            {
                var subdomainService = scope.ServiceProvider.GetRequiredService<ISubdomainService>();
                await subdomainService.SaveTerminalOutputParseAsync(agentRun.Subdomain, agentRun.Agent.Name, agentRun.ActivateNotification, terminalOutputParse, cancellationToken);
            }
        }

        /// <inheritdoc/>
        public async Task UpdateAgentOnScopeAsync(AgentRunner agentRun, string agentRunType, CancellationToken cancellationToken = default)
        {
            using var scope = this.serviceProvider.CreateScope();

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
}