using Microsoft.EntityFrameworkCore;
using NLog;
using ReconNess.Core;
using ReconNess.Core.Managers;
using ReconNess.Core.Models;
using ReconNess.Core.Providers;
using ReconNess.Core.Services;
using ReconNess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Services
{
    /// <summary>
    /// This class implement <see cref="IAgentRunnerService"/>
    /// </summary>
    public class AgentRunnerService : Service<AgentRunner>, IAgentRunnerService, IService<AgentRunner>
    {
        protected static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        private readonly ITargetService targetService;
        private readonly IRootDomainService rootDomainService;
        private readonly ISubdomainService subdomainService;
        private readonly IAgentServerManager agentServerManager;
        private readonly IQueueProvider<AgentRunnerQueue> queueProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="AgentRunnerService" /> class
        /// </summary>
        /// <param name="unitOfWork"><see cref="IUnitOfWork"/></param>
        /// <param name="targetService"><see cref="ITargetService"/></param>
        /// <param name="rootDomainService"><see cref="IRootDomainService"/></param>
        /// <param name="subdomainService"><see cref="ISubdomainService"/></param>
        /// <param name="queueProvider"><see cref="IQueueProvider{T}"/></param>
        /// <param name="agentServerManager"><see cref="IAgentServerManager"/></param>
        public AgentRunnerService(IUnitOfWork unitOfWork,
            ITargetService targetService,
            IRootDomainService rootDomainService,
            ISubdomainService subdomainService,
            IQueueProvider<AgentRunnerQueue> queueProvider,
            IAgentServerManager agentServerManager) : base(unitOfWork)
        {
            this.targetService = targetService;
            this.rootDomainService = rootDomainService;
            this.subdomainService = subdomainService;
            this.queueProvider = queueProvider;
            this.agentServerManager = agentServerManager;
        }

        /// <inheritdoc/>
        public async Task RunAgentAsync(AgentRunnerInfo agentRunnerInfo, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var agentRunnerType = GetAgentRunnerType(agentRunnerInfo);
            if (string.IsNullOrEmpty(agentRunnerType))
            {
                throw new ArgumentException("The Agent does not have a valid Type");
            }

            var agentRunnerSaved = await GetChannelAsync(agentRunnerInfo, cancellationToken);
            if (agentRunnerType.StartsWith("Current"))
            {
                var command = GetCommand(agentRunnerInfo);
                var agentRunnerQueue = new AgentRunnerQueue
                {
                    Channel = agentRunnerSaved.Channel,
                    Command = command,
                    AgentRunnerType = agentRunnerType,
                    Last = true,
                    AllowSkip = false,
                    Count = 1,
                    Total = 1
                };

                await EnqueueRunAgentAsync(agentRunnerQueue, cancellationToken);
            }
            else
            {
                await EnqueueRunAgenthForEachSubConceptAsync(agentRunnerInfo, agentRunnerSaved, agentRunnerType, cancellationToken);
            }
        }

        /// <inheritdoc/>
        public Task StopAgentAsync(string cnannel, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task<List<string>> RunningAgentsAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new List<string>());
        }

        /// <summary>
        /// Run bash for each sublevels
        /// </summary>
        /// <param name="agentRunnerInfo">The agent run parameters</param>
        /// <param name="channel">The channel to send the menssage</param>
        /// <param name="agentRunnerType">The sublevel <see cref="AgentRunnerTypes"/></param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A Task</returns>
        private async Task EnqueueRunAgenthForEachSubConceptAsync(AgentRunnerInfo agentRunnerInfo, AgentRunner agentRunnerSaved, string agentRunnerType, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (AgentRunnerTypes.ALL_TARGETS.Equals(agentRunnerType))
            {
                await EnqueueRunAgenthForEachTargetsAsync(agentRunnerInfo, agentRunnerSaved, cancellationToken);
            }
            else if (AgentRunnerTypes.ALL_ROOTDOMAINS.Equals(agentRunnerType))
            {
                await EnqueueRunAgentForEachRootDomainsAsync(agentRunnerInfo, agentRunnerSaved, cancellationToken);
            }
            else if (AgentRunnerTypes.ALL_SUBDOMAINS.Equals(agentRunnerType))
            {
                await EnqueueRunAgentForEachSubdomainsAsync(agentRunnerInfo, agentRunnerSaved, cancellationToken);
            }
        }

        /// <summary>
        /// Run bash for each Target
        /// </summary>
        /// <param name="agentRunnerInfo">The agent run parameters</param>
        /// <param name="channel">The channel to send the menssage</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A Task</returns>
        private async Task EnqueueRunAgenthForEachTargetsAsync(AgentRunnerInfo agentRunnerInfo, AgentRunner agentRunnerSaved, CancellationToken cancellationToken)
        {
            var targets = await targetService.GetAllAsync(cancellationToken);
            if (!targets.Any())
            {
                agentRunnerSaved.Stage = Entities.Enum.AgentRunStage.SUCCESS;
                await UpdateAsync(agentRunnerSaved);
                return;
            }

            var count = 1;
            foreach (var target in targets)
            {
                var last = count == targets.Count;
                var newAgentRunner = new AgentRunnerInfo
                {
                    Agent = agentRunnerInfo.Agent,
                    Target = target,
                    RootDomain = default,
                    Subdomain = default,
                    ActivateNotification = agentRunnerInfo.ActivateNotification,
                    Command = agentRunnerInfo.Command
                };

                var command = GetCommand(agentRunnerInfo);
                var agentRunnerQueue = new AgentRunnerQueue
                {
                    Channel = agentRunnerSaved.Channel,
                    Command = command,
                    AgentRunnerType = AgentRunnerTypes.ALL_TARGETS,
                    Last = last,
                    AllowSkip = true,
                    Count = count++,
                    Total = targets.Count
                };

                await EnqueueRunAgentAsync(agentRunnerQueue, cancellationToken);
            }
        }

        /// <summary>
        /// Run bash for each Rootdomain
        /// </summary>
        /// <param name="agentRunnerInfo">The agent run parameters</param>
        /// <param name="channel">The channel to send the menssage</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A Task</returns>
        private async Task EnqueueRunAgentForEachRootDomainsAsync(AgentRunnerInfo agentRunnerInfo, AgentRunner agentRunnerSaved, CancellationToken cancellationToken)
        {
            var rootdomains = await rootDomainService.GetAllByCriteriaAsync(r => r.Target == agentRunnerInfo.Target, cancellationToken);
            if (!rootdomains.Any())
            {
                agentRunnerSaved.Stage = Entities.Enum.AgentRunStage.SUCCESS;
                await UpdateAsync(agentRunnerSaved);
                return;
            }

            var count = 1;
            foreach (var rootdomain in rootdomains)
            {
                var last = count == rootdomains.Count;
                var newAgentRunner = new AgentRunnerInfo
                {
                    Agent = agentRunnerInfo.Agent,
                    Target = agentRunnerInfo.Target,
                    RootDomain = rootdomain,
                    Subdomain = default,
                    ActivateNotification = agentRunnerInfo.ActivateNotification,
                    Command = agentRunnerInfo.Command
                };

                var command = GetCommand(agentRunnerInfo);
                var agentRunnerQueue = new AgentRunnerQueue
                {
                    Channel = agentRunnerSaved.Channel,
                    Command = command,
                    AgentRunnerType = AgentRunnerTypes.ALL_ROOTDOMAINS,
                    Last = last,
                    AllowSkip = true,
                    Count = count++,
                    Total = rootdomains.Count
                };

                await EnqueueRunAgentAsync(agentRunnerQueue, cancellationToken);
            }
        }

        /// <summary>
        /// Run bash for each Subdomain
        /// </summary>
        /// <param name="agentRunnerInfo">The agent run parameters</param>
        /// <param name="channel">The channel to send the menssage</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A Task</returns>
        private async Task EnqueueRunAgentForEachSubdomainsAsync(AgentRunnerInfo agentRunnerInfo, AgentRunner agentRunnerSaved, CancellationToken cancellationToken)
        {
            var subdomains = await this.subdomainService.GetSubdomainsNoTrackingAsync(s => s.RootDomain == agentRunner.RootDomain, cancellationToken);
            if (!subdomains.Any())
            {
                agentRunnerSaved.Stage = Entities.Enum.AgentRunStage.SUCCESS;
                await UpdateAsync(agentRunnerSaved);
                return;
            }

            var count = 1;
            foreach (var subdomain in subdomains)
            {
                var last = count == subdomains.Count;
                var newAgentRunner = new AgentRunnerInfo
                {
                    Agent = agentRunnerInfo.Agent,
                    Target = agentRunnerInfo.Target,
                    RootDomain = agentRunnerInfo.RootDomain,
                    Subdomain = subdomain,
                    ActivateNotification = agentRunnerInfo.ActivateNotification,
                    Command = agentRunnerInfo.Command
                };

                var command = GetCommand(agentRunnerInfo);
                var agentRunnerQueue = new AgentRunnerQueue
                {
                    Channel = agentRunnerSaved.Channel,
                    Command = command,
                    AgentRunnerType = AgentRunnerTypes.ALL_SUBDOMAINS,
                    Last = last,
                    AllowSkip = true,
                    Count = count++,
                    Total = subdomains.Count
                };

                await EnqueueRunAgentAsync(agentRunnerQueue, cancellationToken);
            }
        }

        /// <summary>
        /// Run bash
        /// </summary>
        /// <param name="agentRunnerQueue">The agent runner queue information</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        private async Task EnqueueRunAgentAsync(AgentRunnerQueue agentRunnerQueue, CancellationToken cancellationToken = default)
        {
            agentRunnerQueue.AvailableServerNumber = await this.agentServerManager.GetAvailableServerAsync(agentRunnerQueue.Channel, 60, cancellationToken);

            queueProvider.Enqueue(agentRunnerQueue);
        }

        /// <summary>
        /// Obtain the runner id.
        /// 
        /// Ex 
        /// #20220319.1_nmap_yahoo_yahoo.com_www.yahoo.com
        /// #20220318.2_nmap_yahoo_yahoo.com_www.yahoo.com
        /// #20220318.1_nmap_yahoo_yahoo.com_www.yahoo.com
        /// 
        /// </summary>
        /// <param name="agentRunnerInfo">The agent runner</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The agent runner id</returns>
        private async Task<AgentRunner> GetChannelAsync(AgentRunnerInfo agentRunnerInfo, CancellationToken cancellationToken = default)
        {
            string channel = string.Empty;
            if (agentRunnerInfo.Target == null)
            {
                channel = $"{agentRunnerInfo.Agent.Name}";
            }
            else if (agentRunnerInfo.RootDomain == null)
            {
                channel = $"{agentRunnerInfo.Agent.Name}_{agentRunnerInfo.Target.Name}";
            }
            else if (agentRunnerInfo.Subdomain == null)
            {
                channel = $"{agentRunnerInfo.Agent.Name}_{agentRunnerInfo.Target.Name}_{agentRunnerInfo.RootDomain.Name}";
            }
            else
            {
                channel = $"{agentRunnerInfo.Agent.Name}_{agentRunnerInfo.Target.Name}_{agentRunnerInfo.RootDomain.Name}_{agentRunnerInfo.Subdomain.Name}";
            }

            var prefix = DateTime.Now.ToString("yyyyMMdd");
            var count = await GetAllQueryableByCriteria(r => r.Channel.EndsWith(channel) && r.Channel.StartsWith(prefix))
                                .CountAsync(cancellationToken);

            // Ex. #20220319.1_nmap_yahoo_yahho.com_www.yahoo.com
            channel = $"#{prefix}.{++count}_{channel}";

            return await AddAsync(new AgentRunner
            {
                Channel = channel,
                Stage = Entities.Enum.AgentRunStage.ENQUEUE,
                Agent = agentRunnerInfo.Agent
            }, cancellationToken);
        }

        /// <summary>
        /// Obtain the command to run on bash
        /// </summary>
        /// <param name="agentRunnerInfo">The agent</param>
        /// <returns>The command to run on bash</returns>
        private static string GetCommand(AgentRunnerInfo agentRunnerInfo)
        {
            var command = agentRunnerInfo.Command;
            if (string.IsNullOrWhiteSpace(command))
            {
                command = agentRunnerInfo.Agent.Command;
            }

            var envUserName = Environment.GetEnvironmentVariable("ReconnessUserName") ??
                              Environment.GetEnvironmentVariable("ReconnessUserName", EnvironmentVariableTarget.User);

            var envPassword = Environment.GetEnvironmentVariable("ReconnessPassword") ??
                              Environment.GetEnvironmentVariable("ReconnessPassword", EnvironmentVariableTarget.User);

            return command
                .Replace("{{target}}", agentRunnerInfo.Target.Name)
                .Replace("{{rootDomain}}", agentRunnerInfo.RootDomain.Name)
                .Replace("{{rootdomain}}", agentRunnerInfo.RootDomain.Name)
                .Replace("{{domain}}", agentRunnerInfo.Subdomain == null ? agentRunnerInfo.RootDomain.Name : agentRunnerInfo.Subdomain.Name)
                .Replace("{{subdomain}}", agentRunnerInfo.Subdomain == null ? agentRunnerInfo.RootDomain.Name : agentRunnerInfo.Subdomain.Name)
                .Replace("{{userName}}", envUserName)
                .Replace("{{password}}", envPassword)
                .Replace("\"", "\\\"");
        }

        /// <summary>
        /// If we need to run the Agent in each subdomain
        /// </summary>
        /// <param name="agentRunnerInfo">The agent run parameters</param>
        /// <returns>If we need to run the Agent in each subdomain</returns>
        private static string GetAgentRunnerType(AgentRunnerInfo agentRunnerInfo)
        {
            var type = agentRunnerInfo.Agent.AgentType;
            return type switch
            {
                AgentTypes.TARGET => agentRunnerInfo.Target == null ? AgentRunnerTypes.ALL_TARGETS : AgentRunnerTypes.CURRENT_TARGET,
                AgentTypes.ROOTDOMAIN => agentRunnerInfo.RootDomain == null ? AgentRunnerTypes.ALL_ROOTDOMAINS : AgentRunnerTypes.CURRENT_ROOTDOMAIN,
                AgentTypes.SUBDOMAIN => agentRunnerInfo.Subdomain == null ? AgentRunnerTypes.ALL_SUBDOMAINS : AgentRunnerTypes.CURRENT_SUBDOMAIN,
                _ => string.Empty
            };
        }      
    }
}
