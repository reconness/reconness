using NLog;
using ReconNess.Core;
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
    public class AgentRunnerService : Service<Agent>, IAgentRunnerService, IService<Agent>
    {
        protected static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        private readonly ITargetService targetService;
        private readonly IRootDomainService rootDomainService;
        private readonly ISubdomainService subdomainService;
        private readonly IAgentRunnerQueueProvider agentRunnerQueueProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="AgentRunnerService" /> class
        /// </summary>
        /// <param name="unitOfWork"><see cref="IUnitOfWork"/></param>
        /// <param name="targetService"><see cref="ITargetService"/></param>
        /// <param name="rootDomainService"><see cref="IRootDomainService"/></param>
        /// <param name="subdomainService"><see cref="ISubdomainService"/></param>
        /// <param name="agentRunnerQueueProvider"><see cref="IAgentRunnerQueueProvider"/></param>
        public AgentRunnerService(IUnitOfWork unitOfWork,
            ITargetService targetService,
            IRootDomainService rootDomainService,
            ISubdomainService subdomainService,
            IAgentRunnerQueueProvider agentRunnerQueueProvider) : base(unitOfWork)
        {
            this.targetService = targetService;
            this.rootDomainService = rootDomainService;
            this.subdomainService = subdomainService;
            this.agentRunnerQueueProvider = agentRunnerQueueProvider;
        }

        /// <inheritdoc/>
        public async Task RunAgentAsync(Core.Models.AgentRunner agentRunner, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var agentRunnerType = GetAgentRunnerType(agentRunner);
            if (string.IsNullOrEmpty(agentRunnerType))
            {
                throw new ArgumentException("The Agent does not have a valid Type");
            }

            var channel = GetAgentRunnerId(agentRunner);
            if (agentRunnerType.StartsWith("Current"))
            {
                await EnqueueRunAgentAsync(agentRunner, channel, agentRunnerType, last: true, allowSkip: false);
            }
            else
            {
                await EnqueueRunAgenthForEachSubConceptAsync(agentRunner, channel, agentRunnerType, cancellationToken);
            }
        }

        /// <inheritdoc/>
        public Task StopAgentAsync(string channel, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task<List<string>> RunningAgentsAsync(Core.Models.AgentRunner agentRunner, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new List<string>());
        }

        /// <summary>
        /// Run bash for each sublevels
        /// </summary>
        /// <param name="agentRunner">The agent run parameters</param>
        /// <param name="channel">The channel to send the menssage</param>
        /// <param name="agentRunnerType">The sublevel <see cref="AgentRunnerTypes"/></param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A Task</returns>
        private async Task EnqueueRunAgenthForEachSubConceptAsync(Core.Models.AgentRunner agentRunner, string channel, string agentRunnerType, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (AgentRunnerTypes.ALL_TARGETS.Equals(agentRunnerType))
            {
                await EnqueueRunAgenthForEachTargetsAsync(agentRunner, channel, cancellationToken);
            }
            else if (AgentRunnerTypes.ALL_ROOTDOMAINS.Equals(agentRunnerType))
            {
                await EnqueueRunAgentForEachRootDomainsAsync(agentRunner, channel, cancellationToken);
            }
            else if (AgentRunnerTypes.ALL_SUBDOMAINS.Equals(agentRunnerType))
            {
                await EnqueueRunAgentForEachSubdomainsAsync(agentRunner, channel, cancellationToken);
            }
        }

        /// <summary>
        /// Run bash for each Target
        /// </summary>
        /// <param name="agentRunner">The agent run parameters</param>
        /// <param name="channel">The channel to send the menssage</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A Task</returns>
        private async Task EnqueueRunAgenthForEachTargetsAsync(Core.Models.AgentRunner agentRunner, string channel, CancellationToken cancellationToken)
        {
            var targets = await targetService.GetAllAsync(cancellationToken);
            if (!targets.Any())
            {
                // Todo: change runnerId status to Done
                return;
            }

            var targetsCount = targets.Count;
            foreach (var target in targets)
            {
                var last = targetsCount == 1;
                var newAgentRunner = new Core.Models.AgentRunner
                {
                    Agent = agentRunner.Agent,
                    Target = target,
                    RootDomain = default,
                    Subdomain = default,
                    ActivateNotification = agentRunner.ActivateNotification,
                    Command = agentRunner.Command
                };

                await EnqueueRunAgentAsync(newAgentRunner, channel, AgentRunnerTypes.ALL_TARGETS, last);

                targetsCount--;
            }
        }

        /// <summary>
        /// Run bash for each Rootdomain
        /// </summary>
        /// <param name="agentRunner">The agent run parameters</param>
        /// <param name="channel">The channel to send the menssage</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A Task</returns>
        private async Task EnqueueRunAgentForEachRootDomainsAsync(Core.Models.AgentRunner agentRunner, string channel, CancellationToken cancellationToken)
        {
            var rootdomains = await rootDomainService.GetAllByCriteriaAsync(r => r.Target == agentRunner.Target, cancellationToken);
            if (!rootdomains.Any())
            {
                // Todo: change runnerId status to Done
                return;
            }

            var rootdomainsCount = rootdomains.Count;
            foreach (var rootdomain in rootdomains)
            {
                var last = rootdomainsCount == 1;
                var newAgentRunner = new Core.Models.AgentRunner
                {
                    Agent = agentRunner.Agent,
                    Target = agentRunner.Target,
                    RootDomain = rootdomain,
                    Subdomain = default,
                    ActivateNotification = agentRunner.ActivateNotification,
                    Command = agentRunner.Command
                };

                await EnqueueRunAgentAsync(newAgentRunner, channel, AgentRunnerTypes.ALL_ROOTDOMAINS, last);

                rootdomainsCount--;
            }
        }

        /// <summary>
        /// Run bash for each Subdomain
        /// </summary>
        /// <param name="agentRunner">The agent run parameters</param>
        /// <param name="channel">The channel to send the menssage</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A Task</returns>
        private async Task EnqueueRunAgentForEachSubdomainsAsync(Core.Models.AgentRunner agentRunner, string channel, CancellationToken cancellationToken)
        {
            var subdomains = await subdomainService.GetSubdomainsAsync(s => s.RootDomain == agentRunner.RootDomain, cancellationToken);
            if (!subdomains.Any())
            {
                // Todo: change runnerId status to Done
                return;
            }

            var subdomainsCount = subdomains.Count;
            foreach (var subdomain in subdomains)
            {
                var last = subdomainsCount == 1;
                var newAgentRunner = new Core.Models.AgentRunner
                {
                    Agent = agentRunner.Agent,
                    Target = agentRunner.Target,
                    RootDomain = agentRunner.RootDomain,
                    Subdomain = subdomain,
                    ActivateNotification = agentRunner.ActivateNotification,
                    Command = agentRunner.Command
                };

                await EnqueueRunAgentAsync(newAgentRunner, channel, AgentRunnerTypes.ALL_SUBDOMAINS, last);

                subdomainsCount--;
            }
        }

        /// <summary>
        /// Run bash
        /// </summary>
        /// <param name="agentRunner">The agent run parameters</param>
        /// <param name="channel">The channel to send the menssage</param>
        /// <param name="agentRunnerType">The sublevel <see cref="AgentRunnerTypes"/></param>
        /// <param name="last">If is the last bash to run</param>
        /// <returns>A task</returns>
        private async Task EnqueueRunAgentAsync(Core.Models.AgentRunner agentRunner, string channel, string agentRunnerType, bool last, bool allowSkip = true)
        {
            var command = GetCommand(agentRunner);
            await agentRunnerQueueProvider.EnqueueAsync(new AgentRunnerQueue
            {                
                Channel = channel,
                Command = command,
                AgentRunnerType = agentRunnerType,
                Last = last,
                AllowSkip = allowSkip
            });
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
        /// <param name="agentRunner">The agent runner</param>
        /// <returns>The agent runner id</returns>
        private static string GetAgentRunnerId(Core.Models.AgentRunner agentRunner)
        {
            // TODO: obtain from DB the prefix #20220319.1

            if (agentRunner.Target == null)
            {
                return $"{agentRunner.Agent.Name}";
            }

            if (agentRunner.RootDomain == null)
            {
                return $"{agentRunner.Agent.Name}_{agentRunner.Target.Name}";
            }

            if (agentRunner.Subdomain == null)
            {
                return $"{agentRunner.Agent.Name}_{agentRunner.Target.Name}_{agentRunner.RootDomain.Name}";
            }


            // TODO: save runner id, status enqueue

            return $"{agentRunner.Agent.Name}_{agentRunner.Target.Name}_{agentRunner.RootDomain.Name}_{agentRunner.Subdomain.Name}";
        }

        /// <summary>
        /// Obtain the command to run on bash
        /// </summary>
        /// <param name="agentRunner">The agent</param>
        /// <returns>The command to run on bash</returns>
        private static string GetCommand(Core.Models.AgentRunner agentRunner)
        {
            var command = agentRunner.Command;
            if (string.IsNullOrWhiteSpace(command))
            {
                command = agentRunner.Agent.Command;
            }

            var envUserName = Environment.GetEnvironmentVariable("ReconnessUserName") ??
                              Environment.GetEnvironmentVariable("ReconnessUserName", EnvironmentVariableTarget.User);

            var envPassword = Environment.GetEnvironmentVariable("ReconnessPassword") ??
                              Environment.GetEnvironmentVariable("ReconnessPassword", EnvironmentVariableTarget.User);

            return command
                .Replace("{{target}}", agentRunner.Target.Name)
                .Replace("{{rootDomain}}", agentRunner.RootDomain.Name)
                .Replace("{{rootdomain}}", agentRunner.RootDomain.Name)
                .Replace("{{domain}}", agentRunner.Subdomain == null ? agentRunner.RootDomain.Name : agentRunner.Subdomain.Name)
                .Replace("{{subdomain}}", agentRunner.Subdomain == null ? agentRunner.RootDomain.Name : agentRunner.Subdomain.Name)
                .Replace("{{userName}}", envUserName)
                .Replace("{{password}}", envPassword)
                .Replace("\"", "\\\"");
        }

        /// <summary>
        /// If we need to run the Agent in each subdomain
        /// </summary>
        /// <param name="agentRunner">The agent run parameters</param>
        /// <returns>If we need to run the Agent in each subdomain</returns>
        private static string GetAgentRunnerType(Core.Models.AgentRunner agentRunner)
        {
            var type = agentRunner.Agent.AgentType;
            return type switch
            {
                AgentTypes.TARGET => agentRunner.Target == null ? AgentRunnerTypes.ALL_TARGETS : AgentRunnerTypes.CURRENT_TARGET,
                AgentTypes.ROOTDOMAIN => agentRunner.RootDomain == null ? AgentRunnerTypes.ALL_ROOTDOMAINS : AgentRunnerTypes.CURRENT_ROOTDOMAIN,
                AgentTypes.SUBDOMAIN => agentRunner.Subdomain == null ? AgentRunnerTypes.ALL_SUBDOMAINS : AgentRunnerTypes.CURRENT_SUBDOMAIN,
                _ => string.Empty
            };
        }
    }
}
