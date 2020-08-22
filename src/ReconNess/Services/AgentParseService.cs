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
    /// This class implement <see cref="IAgentParseService"/>
    /// </summary>
    public class AgentParseService : IAgentParseService
    {
        private readonly IServiceProvider serviceProvider;

        /// <summary>
        ///  Initializes a new instance of the <see cref="AgentParseService" /> class
        /// </summary>
        /// <param name="serviceProvider"><see cref="IServiceProvider"/></param>
        public AgentParseService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// <see cref="IAgentParseService.SaveScriptOutputAsync(AgentRun, ScriptOutput, CancellationToken)"/>
        /// </summary>
        public async Task SaveScriptOutputAsync(AgentRun agentRun, ScriptOutput scriptOutput, CancellationToken cancellationToken = default)
        {
            using (var scope = this.serviceProvider.CreateScope())
            {
                var rootDomainService =
                    scope.ServiceProvider
                        .GetRequiredService<IRootDomainService>();

                await rootDomainService.SaveScriptOutputAsync(agentRun, scriptOutput, cancellationToken);
            }
        }

        /// <summary>
        /// <see cref="IAgentParseService.UpdateLastRunAsync(Agent, CancellationToken)"/>
        /// </summary>
        public async Task UpdateLastRunAsync(Agent agent, CancellationToken cancellationToken = default)
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
        /// <see cref="IAgentParseService.RegisterAgentAsync(AgentRun, CancellationToken)"/>
        /// </summary>
        public async Task RegisterAgentAsync(AgentRun agentRun, CancellationToken cancellationToken = default)
        {
            using (var scope = this.serviceProvider.CreateScope())
            {
                var subdomainService =
                    scope.ServiceProvider
                        .GetRequiredService<ISubdomainService>();

                await subdomainService.UpdateSubdomainAgent(agentRun.Subdomain, agentRun.Agent.Name, cancellationToken);
            }
        }
    }
}