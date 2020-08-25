using ReconNess.Core;
using ReconNess.Core.Helpers;
using ReconNess.Core.Models;
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
        private readonly IAgentService agentService;
        private readonly IAgentScopeService agentParseService;
        private readonly IScriptEngineService scriptEngineService;
        private readonly ISubdomainService subdomainService;
        private readonly INotificationService notificationService;
        private readonly IConnectorService connectorService;
        private readonly IAgentRunBackgroundTaskQueue backgroundTaskQueue;

        /// <summary>
        /// Initializes a new instance of the <see cref="AgentRunnerService" /> class
        /// </summary>
        /// <param name="unitOfWork"><see cref="IUnitOfWork"/></param>
        /// <param name="agentService"><see cref="IAgentService"/></param>
        /// <param name="agentParseService"><see cref="IAgentScopeService"/></param>
        /// <param name="scriptEngineService"><see cref="IScriptEngineService"/></param>
        /// <param name="subdomainService"><see cref="ISubdomainService"/></param>
        /// <param name="notificationService"><see cref="INotificationService"/></param>
        /// <param name="connectorService"><see cref="IConnectorService"/></param>
        /// <param name="backgroundTaskQueue"><see cref="IAgentRunBackgroundTaskQueue"/></param>
        public AgentRunnerService(IUnitOfWork unitOfWork,
            IAgentService agentService,
            IAgentScopeService agentParseService,
            IScriptEngineService scriptEngineService,
            ISubdomainService subdomainService,
            INotificationService notificationService,
            IConnectorService connectorService,
            IAgentRunBackgroundTaskQueue backgroundTaskQueue) : base(unitOfWork)
        {
            this.agentService = agentService;
            this.agentParseService = agentParseService;
            this.scriptEngineService = scriptEngineService;
            this.subdomainService = subdomainService;
            this.notificationService = notificationService;
            this.connectorService = connectorService;
            this.backgroundTaskQueue = backgroundTaskQueue;
        }

        /// <summary>
        /// <see cref="IAgentRunnerService.RunningAsync(AgentRunner, CancellationToken)"/>
        /// </summary>
        public async Task<List<string>> RunningAsync(AgentRunner agentRunner, CancellationToken cancellationToken = default)
        {
            if (this.backgroundTaskQueue.AgentRunCount == 0)
            {
                return new List<string>();
            }

            var agentsRunning = new List<string>();

            var keys = this.backgroundTaskQueue.AgentRunKeys;
            var agents = await this.agentService.GetAllAsync(cancellationToken);
            foreach (var agent in agents)
            {
                cancellationToken.ThrowIfCancellationRequested();

                agentRunner.Agent = agent;
                var key = AgentRunnerHelpers.GetKey(agentRunner);
                if (keys.Any(c => c.Contains(key)))
                {
                    agentsRunning.Add(agent.Name);
                }
            }

            return agentsRunning;
        }

        /// <summary>
        /// <see cref="IAgentRunnerService.RunAsync(AgentRunner, CancellationToken)"></see>
        /// </summary>
        public async Task RunAsync(AgentRunner agentRunner, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Thread.Sleep(1000);

            this.backgroundTaskQueue.InitializeCurrentAgentRun();

            var channel = AgentRunnerHelpers.GetChannel(agentRunner);
            if (agentRunner.Agent.IsBySubdomain && agentRunner.Subdomain == null)
            {
                await this.RunBashBySubdomainsAsync(agentRunner, channel, cancellationToken);
            }
            else
            {
                await this.RunBashAsync(agentRunner, channel, last: true, removeSubdomainForTheKey: false);
            }
        }

        /// <summary>
        /// <see cref="IAgentRunnerService.StopAsync(AgentRunner, bool, CancellationToken)"></see>
        /// </summary>
        public async Task StopAsync(AgentRunner agentRunner, bool removeSubdomainForTheKey, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            agentRunner.Subdomain = removeSubdomainForTheKey ? null : agentRunner.Subdomain;

            var channel = AgentRunnerHelpers.GetChannel(agentRunner);
            var key = AgentRunnerHelpers.GetKey(agentRunner);

            try
            {
                await this.backgroundTaskQueue.StopCurrentAgentRunAsync(key);
            }
            catch (Exception ex)
            {
                await this.connectorService.SendAsync(channel, ex.Message, cancellationToken);
            }
            finally
            {
                await this.SendAgentDoneNotificationAsync(agentRunner, channel, cancellationToken);
            }
        }

        /// <summary>
        /// Run bash for each subdomain
        /// </summary>
        /// <param name="agentRunner"></param>
        /// <param name="channel">The channel to send the menssage</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task RunBashBySubdomainsAsync(AgentRunner agentRunner, string channel, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var subdomains = await this.subdomainService.GetSubdomainsByRootDomainAsync(agentRunner.RootDomain);
            if (subdomains.Any())
            {
                var subdomainsCount = subdomains.Count;
                foreach (var subdomain in subdomains)
                {
                    var last = subdomainsCount == 1;
                    await this.RunBashAsync(new AgentRunner
                    {
                        Agent = agentRunner.Agent,
                        Target = agentRunner.Target,
                        RootDomain = agentRunner.RootDomain,
                        Subdomain = subdomain,
                        ActivateNotification = agentRunner.ActivateNotification,
                        Command = agentRunner.Command
                    }, channel, last);

                    subdomainsCount--;
                }
            }
            else
            {
                await this.SendAgentDoneNotificationAsync(agentRunner, channel, cancellationToken);
            }
        }

        /// <summary>
        /// Method to run a bash command
        /// </summary>
        /// <param name="agentRunner"></param>
        /// <param name="channel">The channel to send the menssage</param>
        /// <param name="last"></param>
        /// <param name="removeSubdomainForTheKey"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>A Task</returns>
        private Task RunBashAsync(AgentRunner agentRunner, string channel, bool last, bool removeSubdomainForTheKey = true)
        {
            if (this.backgroundTaskQueue.IsCurrentAgentRunStopped())
            {
                return Task.CompletedTask;
            }

            var runnerProcess = new RunnerProcess();
            this.backgroundTaskQueue.QueueAgentRun(new AgentRunnerProcess(AgentRunnerHelpers.GetKey(agentRunner), runnerProcess, async token =>
            {
                try
                {
                    if (AgentRunnerHelpers.NeedToSkipSubdomain(agentRunner))
                    {
                        await this.connectorService.SendLogsAsync(channel, $"Skip subdomain: {agentRunner.Subdomain.Name}", token);
                        await this.connectorService.SendAsync(channel, $"Skip subdomain: {agentRunner.Subdomain.Name} ", token);
                    }
                    else
                    {
                        var command = AgentRunnerHelpers.GetCommand(agentRunner);

                        await this.connectorService.SendLogsAsync(channel, $"RUN: {command}", token);
                        await this.connectorService.SendAsync(channel, $"RUN: {command}", token);

                        runnerProcess.Start(command);

                        int lineCount = 1;
                        var script = agentRunner.Agent.Script;

                        while (!runnerProcess.EndOfStream)
                        {
                            token.ThrowIfCancellationRequested();

                            // Parse the terminal output one line
                            var terminalLineOutput = runnerProcess.TerminalLineOutput();
                            var terminalLineOutputParse = await this.scriptEngineService.TerminalOutputParseAsync(script, terminalLineOutput, lineCount++);

                            await this.connectorService.SendLogsHeadAsync(channel, lineCount, terminalLineOutput, terminalLineOutputParse, token);

                            // Save the Terminal Output Parse 
                            await this.agentParseService.SaveTerminalOutputParseOnScopeAsync(agentRunner, terminalLineOutputParse, token);

                            await this.connectorService.SendLogsTailAsync(channel, lineCount, token);

                            await this.connectorService.SendAsync(channel, terminalLineOutput, token, false);
                        }

                        if (agentRunner.Subdomain != null)
                        {
                            await this.agentParseService.UpdateSubdomainAgentOnScopeAsync(agentRunner, token);
                        }
                    }
                }
                catch (Exception ex)
                {
                    await this.connectorService.SendAsync(channel, ex.Message, token);
                    await this.connectorService.SendLogsAsync(channel, $"Exception: {ex.StackTrace}", token);
                }
                finally
                {
                    if (last)
                    {
                        try
                        {
                            await this.StopAsync(agentRunner, removeSubdomainForTheKey, token);
                            await this.agentParseService.UpdateLastRunAgentOnScopeAsync(agentRunner.Agent, token);
                        }
                        catch (Exception exx)
                        {
                            await this.connectorService.SendLogsAsync(channel, $"Exception: {exx.StackTrace}", token);
                        }
                    }
                }
            }));

            return Task.CompletedTask;
        }

        /// <summary>
        /// Send a msg and a notification when the agent finish
        /// </summary>
        /// <param name="agent">The agent</param>
        /// <param name="channel">The channel to use to send the msg</param>
        /// <param name="activateNotification">If we need to send a notification</param> 
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task SendAgentDoneNotificationAsync(AgentRunner agentRunner, string channel, CancellationToken cancellationToken)
        {
            if (agentRunner.ActivateNotification && agentRunner.Agent.NotifyIfAgentDone)
            {
                await this.notificationService.SendAsync($"Agent {agentRunner.Agent.Name} is done!", cancellationToken);
            }

            await this.connectorService.SendAsync(channel, "Agent done!", cancellationToken);
        }
    }
}
