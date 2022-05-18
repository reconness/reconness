using Microsoft.EntityFrameworkCore;
using NLog;
using ReconNess.Core;
using ReconNess.Core.Managers;
using ReconNess.Core.Models;
using ReconNess.Core.Providers;
using ReconNess.Core.Services;
using ReconNess.Entities;
using ReconNess.Entities.Enum;
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
            if (agentRunnerType.StartsWith("Current"))
            {
                await EnqueueAgentRunnerCurrentConceptAsync(agentRunnerInfo, agentRunnerType, cancellationToken);
            }
            else
            {
                await EnqueueAgentRunnerForEachSubConceptAsync(agentRunnerInfo, agentRunnerType, cancellationToken);
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
        /// Enqueue current concept [target, rootdomain, subdomain]
        /// </summary>
        /// <param name="agentRunnerInfo">The agent run parameters</param>
        /// <param name="agentRunnerType">The sublevel <see cref="AgentRunnerTypes"/></param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        private async Task EnqueueAgentRunnerCurrentConceptAsync(AgentRunnerInfo agentRunnerInfo, string agentRunnerType, CancellationToken cancellationToken)
        {
            var channel = await GetChannelAsync(agentRunnerInfo, cancellationToken);
            await AddAsync(new AgentRunner
            {
                Channel = channel,
                Stage = AgentRunnerStage.ENQUEUE,
                AllowSkip = false,
                Total = 1,
                AgentRunnerType = agentRunnerType,
                ActivateNotification = agentRunnerInfo.ActivateNotification,
                Agent = agentRunnerInfo.Agent
            }, cancellationToken);

            var command = GetCommand(agentRunnerInfo);
            var agentRunnerQueue = new AgentRunnerQueue
            {
                Channel = channel,
                Command = command,
                Count = 1
            };

            await EnqueueRunAgentAsync(agentRunnerQueue, cancellationToken);
        }

        /// <summary>
        /// Enqueue for each sub concept [target, rootdomain, subdomain]
        /// </summary>
        /// <param name="agentRunnerInfo">The agent run parameters</param>
        /// <param name="agentRunnerType">The sublevel <see cref="AgentRunnerTypes"/></param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A Task</returns>
        private async Task EnqueueAgentRunnerForEachSubConceptAsync(AgentRunnerInfo agentRunnerInfo, string agentRunnerType, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var channel = await GetChannelAsync(agentRunnerInfo, cancellationToken);
            if (AgentRunnerTypes.ALL_TARGETS.Equals(agentRunnerType))
            {
                await EnqueueRunAgenthForEachTargetsAsync(agentRunnerInfo, channel, cancellationToken);
            }
            else if (AgentRunnerTypes.ALL_ROOTDOMAINS.Equals(agentRunnerType))
            {
                await EnqueueRunAgentForEachRootDomainsAsync(agentRunnerInfo, channel, cancellationToken);
            }
            else if (AgentRunnerTypes.ALL_SUBDOMAINS.Equals(agentRunnerType))
            {
                await EnqueueRunAgentForEachSubdomainsAsync(agentRunnerInfo, channel, cancellationToken);
            }
        }

        /// <summary>
        /// Run bash for each Target
        /// </summary>
        /// <param name="agentRunnerInfo">The agent run parameters</param>
        /// <param name="channel">The channel to send the menssage</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A Task</returns>
        private async Task EnqueueRunAgenthForEachTargetsAsync(AgentRunnerInfo agentRunnerInfo, string channel, CancellationToken cancellationToken)
        {
            var targets = await targetService.GetAllAsync(cancellationToken);
            var stage = targets.Any() ? AgentRunnerStage.ENQUEUE : AgentRunnerStage.SUCCESS;

            await AddAsync(new AgentRunner
            {
                Channel = channel,
                Stage = stage,
                AllowSkip = true,
                Total = targets.Count,
                ActivateNotification = agentRunnerInfo.ActivateNotification,
                AgentRunnerType = AgentRunnerTypes.ALL_TARGETS,
                Agent = agentRunnerInfo.Agent
            }, cancellationToken);

            var count = 1;
            foreach (var target in targets)
            {
                agentRunnerInfo.Target = target;
                var command = GetCommand(agentRunnerInfo);

                var agentRunnerQueue = new AgentRunnerQueue
                {
                    Channel = channel,
                    Command = command,
                    Count = count++,
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
        private async Task EnqueueRunAgentForEachRootDomainsAsync(AgentRunnerInfo agentRunnerInfo, string channel, CancellationToken cancellationToken)
        {
            var rootdomains = await rootDomainService.GetAllByCriteriaAsync(r => r.Target == agentRunnerInfo.Target, cancellationToken);
            var stage = rootdomains.Any() ? AgentRunnerStage.ENQUEUE : AgentRunnerStage.SUCCESS;

            await AddAsync(new AgentRunner
            {
                Channel = channel,
                Stage = stage,
                AllowSkip = true,
                Total = rootdomains.Count,
                ActivateNotification = agentRunnerInfo.ActivateNotification,
                AgentRunnerType = AgentRunnerTypes.ALL_ROOTDOMAINS,
                Agent = agentRunnerInfo.Agent
            }, cancellationToken);

            var count = 1;
            foreach (var rootdomain in rootdomains)
            {
                agentRunnerInfo.RootDomain = rootdomain;
                var command = GetCommand(agentRunnerInfo);

                var agentRunnerQueue = new AgentRunnerQueue
                {
                    Channel = channel,
                    Command = command,
                    Count = count++
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
        private async Task EnqueueRunAgentForEachSubdomainsAsync(AgentRunnerInfo agentRunnerInfo, string channel, CancellationToken cancellationToken)
        {
            var subdomains = await this.subdomainService.GetSubdomainsNoTrackingAsync(s => s.RootDomain == agentRunnerInfo.RootDomain, cancellationToken);
            var stage = subdomains.Any() ? AgentRunnerStage.ENQUEUE : AgentRunnerStage.SUCCESS;            

            await AddAsync(new AgentRunner
            {
                Channel = channel,
                Stage = stage,
                AllowSkip = true,
                Total = subdomains.Count,
                AgentRunnerType = AgentRunnerTypes.ALL_SUBDOMAINS,
                ActivateNotification = agentRunnerInfo.ActivateNotification,
                Agent = agentRunnerInfo.Agent
            }, cancellationToken);

            var count = 1;
            foreach (var subdomain in subdomains)
            {
                agentRunnerInfo.Subdomain = subdomain;
                var command = GetCommand(agentRunnerInfo);

                var agentRunnerQueue = new AgentRunnerQueue
                {
                    Channel = channel,
                    Command = command,
                    Count = count++
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
        /// Obtain the channel.
        /// 
        /// Ex 
        /// #20220319.1_nmap_yahoo_yahoo.com_www.yahoo.com
        /// #20220318.2_nmap_yahoo_yahoo.com_www.yahoo.com
        /// #20220318.1_nmap_yahoo_yahoo.com_www.yahoo.com
        /// 
        /// </summary>
        /// <param name="agentRunnerInfo">The agent runner</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The agent runner channel</returns>
        private async Task<string> GetChannelAsync(AgentRunnerInfo agentRunnerInfo, CancellationToken cancellationToken = default)
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

            return channel;
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
        /// <exception cref="ArgumentException">If the Agent does not have a valid Type</exception>
        private static string GetAgentRunnerType(AgentRunnerInfo agentRunnerInfo)
        {
            var type = agentRunnerInfo.Agent.AgentType;
            return type switch
            {
                AgentTypes.TARGET => agentRunnerInfo.Target == null ? AgentRunnerTypes.ALL_TARGETS : AgentRunnerTypes.CURRENT_TARGET,
                AgentTypes.ROOTDOMAIN => agentRunnerInfo.RootDomain == null ? AgentRunnerTypes.ALL_ROOTDOMAINS : AgentRunnerTypes.CURRENT_ROOTDOMAIN,
                AgentTypes.SUBDOMAIN => agentRunnerInfo.Subdomain == null ? AgentRunnerTypes.ALL_SUBDOMAINS : AgentRunnerTypes.CURRENT_SUBDOMAIN,
                _ => throw new ArgumentException("The Agent does not have a valid Type")
            };
        }      
    }
}
