using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using ReconNess.Core;
using ReconNess.Core.Models;
using ReconNess.Core.Services;
using ReconNess.Entities;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Services
{
    /// <summary>
    /// This class implement <see cref="IAgentRunService"/>
    /// </summary>
    public class AgentRunService : IAgentRunService
    {
        protected static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        private readonly IServiceProvider serviceProvider;
        private readonly IConnectorService connectorService;

        private static ConcurrentDictionary<string, string> terminalOuputs = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AgentRunService" /> class
        /// </summary>
        /// <param name="unitOfWork"><see cref="IUnitOfWork"/></param>
        /// <param name="serviceProvider"><see cref="IServiceProvider"/></param>
        /// <param name="connectorService"><see cref="IConnectorService"/></param>
        public AgentRunService(
            IServiceProvider serviceProvider,
            IConnectorService connectorService)
        {
            this.serviceProvider = serviceProvider;
            this.connectorService = connectorService;
        }

        /// <summary>
        /// <see cref="IAgentRunService.StartOnScopeAsync(AgentRunner, string, CancellationToken)"/>
        /// </summary>
        public async Task StartOnScopeAsync(AgentRunner agentRunner, string channel, CancellationToken cancellationToken = default)
        {
            var agentRun = new AgentRun
            {
                Agent = agentRunner.Agent,
                Channel = channel,
                Description = $"Start running the agent {agentRunner.Agent.Name}",
                Stage = Entities.Enum.AgentRunStage.RUNNING
            };

            using (var scope = this.serviceProvider.CreateScope())
            {
                var unitOfWork =
                    scope.ServiceProvider
                        .GetRequiredService<IUnitOfWork>();

                unitOfWork.Repository<AgentRun>().Update(agentRun);
                await unitOfWork.CommitAsync();
            }
        }

        /// <summary>
        /// <see cref="IAgentRunService.UpdateAgentRunDoneOnScopeAsync(Agent, string, bool, bool, CancellationToken)"/>
        /// </summary>
        public async Task DoneOnScopeAsync(AgentRunner agentRunner, string channel, bool stoppedManually, bool fromException, CancellationToken cancellationToken)
        {
            if (agentRunner.ActivateNotification)
            {
                await this.SendNotificationOnScopeAsync($"Agent {agentRunner.Agent.Name} is done!", cancellationToken);
            }

            await this.UpdateLastRunAgentOnScopeAsync(agentRunner.Agent, cancellationToken);
            

            using (var scope = this.serviceProvider.CreateScope())
            {
                var unitOfWork =
                    scope.ServiceProvider
                        .GetRequiredService<IUnitOfWork>();

                var agentRun = await unitOfWork.Repository<AgentRun>()
                        .GetAllQueryableByCriteria(ar => ar.Agent == agentRunner.Agent && ar.Channel == channel)
                        .OrderByDescending(o => o.CreatedAt)
                    .SingleOrDefaultAsync(cancellationToken);

                if (agentRun != null)
                {
                    if (fromException)
                    {
                        agentRun.Stage = Entities.Enum.AgentRunStage.FAILED;
                        agentRun.Description = $"The agent {agentRunner.Agent.Name} failed";
                    }
                    else if (stoppedManually)
                    {
                        agentRun.Stage = Entities.Enum.AgentRunStage.STOPPED;
                        agentRun.Description = $"The agent {agentRunner.Agent.Name} was stopped manually";
                    }
                    else
                    {
                        agentRun.Stage = Entities.Enum.AgentRunStage.SUCCESS;
                        agentRun.Description = $"The agent {agentRunner.Agent.Name} ran successfully";
                    }

                    if (terminalOuputs.ContainsKey(channel))
                    {
                        agentRun.TerminalOutput = terminalOuputs[channel];
                        terminalOuputs[channel] = string.Empty;
                    }

                    agentRun.TerminalOutput += "\nAgent Done";

                    unitOfWork.Repository<AgentRun>().Update(agentRun);
                    await unitOfWork.CommitAsync();
                }
            }

            await this.connectorService.SendAsync(channel, "Agent Done!", false, cancellationToken);      
        }

        /// <summary>
        /// <see cref="IAgentRunService.TerminalOutputScopeAsync(AgentRunner, string, string, bool, CancellationToken)"/>
        /// </summary>
        public async Task TerminalOutputScopeAsync(AgentRunner agentRunner, string channel, string terminalOutput, bool includeTime = true, CancellationToken cancellationToken = default)
        {            
            if (!terminalOuputs.ContainsKey(channel))
            {
                terminalOuputs.TryAdd(channel, string.Empty);
            }

            terminalOuputs[channel] += terminalOutput;

            await connectorService.SendAsync(channel, terminalOutput, includeTime, cancellationToken);
        }

        /// <summary>
        /// Update the latest agent run date
        /// </summary>
        /// <param name="agent">The agent</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        private async Task UpdateLastRunAgentOnScopeAsync(Agent agent, CancellationToken cancellationToken = default)
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
        /// Send a notification 
        /// </summary>
        /// <param name="payload">The payload to send</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        private async Task SendNotificationOnScopeAsync(string payload, CancellationToken cancellationToken = default)
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
