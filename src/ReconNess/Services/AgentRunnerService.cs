using NLog;
using ReconNess.Core;
using ReconNess.Core.Models;
using ReconNess.Core.Providers;
using ReconNess.Core.Services;
using ReconNess.Entities;
using ReconNess.Helpers;
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

        private readonly IAgentService agentService;
        private readonly ITargetService targetService;
        private readonly IRootDomainService rootDomainService;
        private readonly ISubdomainService subdomainService;
        private readonly IAgentRunnerProvider agentRunnerProvider;
        private readonly IAgentBackgroundService agentBackgroundService;
        private readonly IAgentRunService agentRunService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AgentRunnerService" /> class
        /// </summary>
        /// <param name="unitOfWork"><see cref="IUnitOfWork"/></param>
        /// <param name="agentService"><see cref="IAgentService"/></param>
        /// <param name="targetService"><see cref="ITargetService"/></param>
        /// <param name="rootDomainService"><see cref="IRootDomainService"/></param>
        /// <param name="subdomainService"><see cref="ISubdomainService"/></param>
        /// <param name="agentRunnerProvider"><see cref="IAgentRunnerProvider"/></param>
        /// <param name="agentBackgroundService"><see cref="IAgentBackgroundService"/></param>
        /// <param name="agentRunService"><see cref="IAgentRunService"/></param>
        public AgentRunnerService(IUnitOfWork unitOfWork,
            IAgentService agentService,
            ITargetService targetService,
            IRootDomainService rootDomainService,
            ISubdomainService subdomainService,
            IAgentRunnerProvider agentRunnerProvider,
            IAgentBackgroundService agentBackgroundService,
            IAgentRunService agentRunService) : base(unitOfWork)
        {
            this.agentService = agentService;
            this.targetService = targetService;
            this.rootDomainService = rootDomainService;
            this.subdomainService = subdomainService;
            this.agentRunnerProvider = agentRunnerProvider;
            this.agentBackgroundService = agentBackgroundService;
            this.agentRunService = agentRunService;
        }

        /// <summary>
        /// <see cref="IAgentRunnerService.RunningAgentsAsync(AgentRunner, CancellationToken)"/>
        /// </summary>
        public async Task<List<string>> RunningAgentsAsync(AgentRunner agentRunner, CancellationToken cancellationToken = default)
        {
            if ((await this.agentRunnerProvider.RunningCountAsync) == 0)
            {
                return new List<string>();
            }

            var agentsRunning = new List<string>();

            var channels = await this.agentRunnerProvider.RunningChannelsAsync;

            var agents = await this.agentService.GetAllAsync(cancellationToken);
            foreach (var agent in agents)
            {
                cancellationToken.ThrowIfCancellationRequested();

                agentRunner.Agent = agent;
                var channel = AgentRunnerHelpers.GetChannel(agentRunner);

                if (channels.Any(c => c.Contains(channel)))
                {
                    agentsRunning.Add(agent.Name);
                }
            }

            return agentsRunning;
        }

        /// <summary>
        /// <see cref="IAgentRunnerService.RunAgentAsync(AgentRunner, CancellationToken)"></see>
        /// </summary>
        public async Task RunAgentAsync(AgentRunner agentRunner, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var channel = AgentRunnerHelpers.GetChannel(agentRunner);
            await this.agentRunnerProvider.InitializesAsync(channel);

            _logger.Info($"Start channel {channel}");

            await this.agentRunService.StartOnScopeAsync(agentRunner, channel, cancellationToken);

            var agentRunnerType = this.GetAgentRunnerType(agentRunner);
            if (agentRunnerType.StartsWith("Current"))
            {
                await this.RunAgentAsync(agentRunner, channel, agentRunnerType, last: true, allowSkip: false);
            }
            else
            {
                await this.RunAgenthInEachSubConceptAsync(agentRunner, channel, agentRunnerType, cancellationToken);
            }
        }

        /// <summary>
        /// <see cref="IAgentRunnerService.StopAgentAsync(AgentRunner,  CancellationToken)"></see>
        /// </summary>
        public async Task StopAgentAsync(AgentRunner agentRunner, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var channel = AgentRunnerHelpers.GetChannel(agentRunner);

            await this.StopAgentAsync(channel, cancellationToken);
            await this.agentRunService.DoneOnScopeAsync(agentRunner, channel, true, false, cancellationToken);
        }

        /// <summary>
        /// Stop the agent if it is running
        /// </summary>
        /// <param name="channel">The channel</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        private async Task StopAgentAsync(string channel, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                if (!(await this.agentRunnerProvider.IsStoppedAsync(channel)))
                {
                    _logger.Info($"Stop channel {channel}");

                    await this.agentRunnerProvider.StopAsync(channel);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
            }
        }

        /// <summary>
        /// If we need to run the Agent in each subdomain
        /// </summary>
        /// <param name="agentRunner">The agent run parameters</param>
        /// <returns>If we need to run the Agent in each subdomain</returns>
        private string GetAgentRunnerType(AgentRunner agentRunner)
        {
            var type = agentRunner.Agent.AgentType;
            return type switch
            {
                AgentTypes.TARGET => agentRunner.Target == null ? AgentRunnerTypes.ALL_TARGETS : AgentRunnerTypes.CURRENT_TARGET,
                AgentTypes.ROOTDOMAIN => agentRunner.RootDomain == null ? AgentRunnerTypes.ALL_ROOTDOMAINS : AgentRunnerTypes.CURRENT_ROOTDOMAIN,
                AgentTypes.SUBDOMAIN => agentRunner.Subdomain == null ? AgentRunnerTypes.ALL_SUBDOMAINS : AgentRunnerTypes.CURRENT_SUBDOMAIN,
                _ => throw new ArgumentException("The Agent need to have valid Type")
            };
        }

        /// <summary>
        /// Run bash for each sublevels
        /// </summary>
        /// <param name="agentRunner">The agent run parameters</param>
        /// <param name="channel">The channel to send the menssage</param>
        /// <param name="agentRunnerType">The sublevel <see cref="AgentRunnerTypes"/></param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A Task</returns>
        private async Task RunAgenthInEachSubConceptAsync(AgentRunner agentRunner, string channel, string agentRunnerType, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (AgentRunnerTypes.ALL_TARGETS.Equals(agentRunnerType))
            {
                await this.RunAgenthInEachTargetsAsync(agentRunner, channel, cancellationToken);
            }
            else if (AgentRunnerTypes.ALL_ROOTDOMAINS.Equals(agentRunnerType))
            {
                await this.RunAgentInEachRootDomainsAsync(agentRunner, channel, cancellationToken);
            }
            else if (AgentRunnerTypes.ALL_SUBDOMAINS.Equals(agentRunnerType))
            {
                await this.RunAgentInEachSubdomainsAsync(agentRunner, channel, cancellationToken);
            }
        }

        /// <summary>
        /// Run bash for each Target
        /// </summary>
        /// <param name="agentRunner">The agent run parameters</param>
        /// <param name="channel">The channel to send the menssage</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A Task</returns>
        private async Task RunAgenthInEachTargetsAsync(AgentRunner agentRunner, string channel, CancellationToken cancellationToken)
        {
            var targets = await this.targetService.GetAllAsync(cancellationToken);
            if (!targets.Any())
            {
                await this.agentRunService.DoneOnScopeAsync(agentRunner, channel, false, false, cancellationToken);
                return;
            }

            var targetsCount = targets.Count;
            foreach (var target in targets)
            {
                var last = targetsCount == 1;
                var newAgentRunner = new AgentRunner
                {
                    Agent = agentRunner.Agent,
                    Target = target,
                    RootDomain = default,
                    Subdomain = default,
                    ActivateNotification = agentRunner.ActivateNotification,
                    Command = agentRunner.Command
                };

                await this.RunAgentAsync(newAgentRunner, channel, AgentRunnerTypes.ALL_TARGETS, last, allowSkip: true);

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
        private async Task RunAgentInEachRootDomainsAsync(AgentRunner agentRunner, string channel, CancellationToken cancellationToken)
        {
            var rootdomains = await this.rootDomainService.GetAllByCriteriaAsync(r => r.Target == agentRunner.Target, cancellationToken);
            if (!rootdomains.Any())
            {
                await this.agentRunService.DoneOnScopeAsync(agentRunner, channel, false, false, cancellationToken);
                return;
            }

            var rootdomainsCount = rootdomains.Count;
            foreach (var rootdomain in rootdomains)
            {
                var last = rootdomainsCount == 1;
                var newAgentRunner = new AgentRunner
                {
                    Agent = agentRunner.Agent,
                    Target = agentRunner.Target,
                    RootDomain = rootdomain,
                    Subdomain = default,
                    ActivateNotification = agentRunner.ActivateNotification,
                    Command = agentRunner.Command
                };

                await this.RunAgentAsync(newAgentRunner, channel, AgentRunnerTypes.ALL_ROOTDOMAINS, last, allowSkip: true);

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
        private async Task RunAgentInEachSubdomainsAsync(AgentRunner agentRunner, string channel, CancellationToken cancellationToken)
        {
            var subdomains = await this.subdomainService.GetSubdomainsAsync(s => s.RootDomain == agentRunner.RootDomain, cancellationToken);
            if (!subdomains.Any())
            {
                await this.agentRunService.DoneOnScopeAsync(agentRunner, channel, false, false, cancellationToken);
                return;
            }

            var subdomainsCount = subdomains.Count;
            foreach (var subdomain in subdomains)
            {
                var last = subdomainsCount == 1;
                var newAgentRunner = new AgentRunner
                {
                    Agent = agentRunner.Agent,
                    Target = agentRunner.Target,
                    RootDomain = agentRunner.RootDomain,
                    Subdomain = subdomain,
                    ActivateNotification = agentRunner.ActivateNotification,
                    Command = agentRunner.Command
                };

                await this.RunAgentAsync(newAgentRunner, channel, AgentRunnerTypes.ALL_SUBDOMAINS, last, allowSkip: true);

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
        private async Task RunAgentAsync(AgentRunner agentRunner, string channel, string agentRunnerType, bool last, bool allowSkip)
        {
            if (await this.agentRunnerProvider.IsStoppedAsync(channel))
            {
                return;
            }

            var command = AgentRunnerHelpers.GetCommand(agentRunner);
            await this.agentRunnerProvider.RunAsync(new AgentRunnerProviderArgs
            {
                AgentRunner = agentRunner,
                Channel = channel,
                Command = command,
                AgentRunnerType = agentRunnerType,
                Last = last,
                AllowSkip = allowSkip,
                BeginHandlerAsync = BeginHandlerAsync,
                SkipHandlerAsync = SkipHandlerAsync,
                ParserOutputHandlerAsync = ParserOutputHandlerAsync,
                EndHandlerAsync = EndHandlerAsync,
                ExceptionHandlerAsync = ExceptionHandlerAsync
            });
        }

        /// <summary>
        /// <see cref="IAgentRunnerProvider.SkipHandlerAsync"/>
        /// </summary>
        private async Task<bool> SkipHandlerAsync(AgentRunnerProviderResult result)
        {
            if (AgentRunnerHelpers.NeedToSkipRun(result.AgentRunner, result.AgentRunnerType))
            {
                await this.agentRunService.TerminalOutputScopeAsync(result.AgentRunner, result.Channel, $"SKIP: {result.Command}", includeTime: true, result.CancellationToken);

                return true;
            }

            return false;
        }

        /// <summary>
        /// <see cref="IAgentRunnerProvider.BeginHandlerAsync"/>
        /// </summary>
        private async Task BeginHandlerAsync(AgentRunnerProviderResult result)
        {
            _logger.Info($"Start command {result.Command}");

            await this.agentRunService.TerminalOutputScopeAsync(result.AgentRunner, result.Channel, $"RUN: {result.Command}", includeTime: true, result.CancellationToken);
        }

        /// <summary>
        /// <see cref="IAgentRunnerProvider.ParserOutputHandlerAsync"/>
        /// </summary>
        private async Task ParserOutputHandlerAsync(AgentRunnerProviderResult result)
        {
            // Save the Terminal Output Parse 
            await this.agentBackgroundService.SaveOutputParseOnScopeAsync
            (
                result.AgentRunner,
                result.AgentRunnerType,
                result.ScriptOutput,
                result.CancellationToken
            );

            await this.agentRunService.TerminalOutputScopeAsync(result.AgentRunner, result.Channel, result.TerminalLineOutput, includeTime: false, result.CancellationToken);
        }

        /// <summary>
        /// <see cref="IAgentRunnerProvider.EndHandlerAsync"/>
        /// </summary>
        private async Task EndHandlerAsync(AgentRunnerProviderResult result)
        {
            _logger.Info($"End command {result.Command}");

            await this.agentBackgroundService.UpdateAgentOnScopeAsync(result.AgentRunner, result.AgentRunnerType, result.CancellationToken);

            if (result.Last)
            {
                await this.StopAgentAsync(result.Channel, result.CancellationToken);
                await this.agentRunService.DoneOnScopeAsync(result.AgentRunner, result.Channel, false, false, result.CancellationToken);
            }
        }

        /// <summary>
        /// <see cref="IAgentRunnerProvider.ExceptionHandlerAsync"/>
        /// </summary>
        private async Task ExceptionHandlerAsync(AgentRunnerProviderResult result)
        {
            _logger.Error(result.Exception, $"Exception running command {result.Command}");

            await this.agentRunService.TerminalOutputScopeAsync(result.AgentRunner, result.Channel, result.Exception.Message, includeTime: true, result.CancellationToken);

            if (result.Last)
            {
                await this.StopAgentAsync(result.Channel, result.CancellationToken);
                await this.agentRunService.DoneOnScopeAsync(result.AgentRunner, result.Channel, false, true, result.CancellationToken);
            }
        }
    }
}
