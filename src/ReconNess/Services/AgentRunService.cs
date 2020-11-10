using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using ReconNess.Core;
using ReconNess.Core.Models;
using ReconNess.Core.Services;
using ReconNess.Entities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Services
{
    /// <summary>
    /// This class implement <see cref="IAgentRunService"/>
    /// </summary>
    public class AgentRunService : Service<AgentRun>, IService<AgentRun>, IAgentRunService
    {
        protected static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        private readonly IServiceProvider serviceProvider;
        private readonly IConnectorService connectorService;
        /// <summary>
        /// Initializes a new instance of the <see cref="AgentRunService" /> class
        /// </summary>
        /// <param name="unitOfWork"><see cref="IUnitOfWork"/></param>
        public AgentRunService(IUnitOfWork unitOfWork,
            IServiceProvider serviceProvider,
            IConnectorService connectorService)
            : base(unitOfWork)
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

            await this.UpdateAsync(agentRun);
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

            var agentRun = await this.GetLastAgentRunAsync(agentRunner, channel, cancellationToken);
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

                agentRun.Logs += "\nAgent Done";
                agentRun.TerminalOutput += "\nAgent Done";

                using (var scope = this.serviceProvider.CreateScope())
                {
                    var unitOfWork =
                        scope.ServiceProvider
                            .GetRequiredService<IUnitOfWork>();

                    unitOfWork.Repository<AgentRun>().Update(agentRun);
                    await unitOfWork.CommitAsync();
                }

                await this.connectorService.SendAsync(channel, "\nAgent Done", false, cancellationToken);
                await this.connectorService.SendLogsAsync(channel, "\nAgent Done", cancellationToken);
            }
        }        

        /// <summary>
        /// <see cref="IAgentRunService.InsertTerminalScopeAsync(AgentRunner, string, string, bool, CancellationToken)"/>
        /// </summary>
        public async Task InsertTerminalScopeAsync(AgentRunner agentRunner, string channel, string terminalOutput, bool includeTime = true, CancellationToken cancellationToken = default)
        {
            var agentRun = await this.GetLastAgentRunAsync(agentRunner, channel, cancellationToken);
            if (agentRun != null)
            {
                using (var scope = this.serviceProvider.CreateScope())
                {
                    var unitOfWork =
                        scope.ServiceProvider
                            .GetRequiredService<IUnitOfWork>();

                    agentRun.TerminalOutput += $"\n{terminalOutput}";

                    unitOfWork.Repository<AgentRun>().Update(agentRun);
                    await unitOfWork.CommitAsync();
                }

                await this.connectorService.SendAsync(channel, terminalOutput, includeTime, cancellationToken);
            }
        }

        /// <summary>
        /// <see cref="IAgentRunService.InsertLogsScopeAsync(AgentRunner, string, string, CancellationToken)"/>
        /// </summary>
        public async Task InsertLogsScopeAsync(AgentRunner agentRunner, string channel, string logs, CancellationToken cancellationToken)
        {
            var agentRun = await this.GetLastAgentRunAsync(agentRunner, channel, cancellationToken);
            if (agentRun != null)
            {
                using (var scope = this.serviceProvider.CreateScope())
                {
                    var unitOfWork =
                        scope.ServiceProvider
                            .GetRequiredService<IUnitOfWork>();

                    agentRun.Logs += $"\n{logs}";

                    unitOfWork.Repository<AgentRun>().Update(agentRun);
                    await unitOfWork.CommitAsync();
                }

                await this.connectorService.SendLogsAsync(channel, logs, cancellationToken);
            }
        }

        /// <summary>
        /// Obtain the last agent running base on the channel
        /// </summary>
        /// <param name="agentRunner">The agent running</param>
        /// <param name="channel">The channel</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The last agent running base on the channel</returns>
        private async Task<AgentRun> GetLastAgentRunAsync(AgentRunner agentRunner, string channel, CancellationToken cancellationToken)
        {
            using (var scope = this.serviceProvider.CreateScope())
            {
                var unitOfWork =
                    scope.ServiceProvider
                        .GetRequiredService<IUnitOfWork>();

                return await unitOfWork.Repository<AgentRun>()
                    .GetAllQueryableByCriteria(ar => ar.Agent == agentRunner.Agent && ar.Channel == channel)
                    .OrderBy(o => o.CreatedAt)
                    .FirstOrDefaultAsync(cancellationToken);
            }                            
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
