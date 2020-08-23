using Newtonsoft.Json;
using ReconNess.Core;
using ReconNess.Core.Helpers;
using ReconNess.Core.Models;
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
        private readonly IConnectorService connectorService;
        private readonly IScriptEngineService scriptEngineService;
        private readonly INotificationService notificationService;
        private readonly IAgentParseService agentParseService;
        private readonly IAgentRunBackgroundTaskQueue backgroundTaskQueue;

        /// <summary>
        /// Initializes a new instance of the <see cref="AgentRunnerService" /> class
        /// </summary>
        /// <param name="unitOfWork"><see cref="IUnitOfWork"/></param>
        /// <param name="connectorService"><see cref="IConnectorService"/></param>
        /// <param name="scriptEngineService"><see cref="IScriptEngineService"/></param>
        /// <param name="notificationService"><see cref="INotificationService"/></param>
        /// <param name="agentParseService"><see cref="IAgentParseService"/></param>
        /// <param name="backgroundTaskQueue"><see cref="IAgentRunBackgroundTaskQueue"/></param>
        public AgentRunnerService(IUnitOfWork unitOfWork,
            IConnectorService connectorService,
            IScriptEngineService scriptEngineService,
            INotificationService notificationService,
            IAgentParseService agentParseService,
            IAgentRunBackgroundTaskQueue backgroundTaskQueue) : base(unitOfWork)
        {
            this.connectorService = connectorService;
            this.scriptEngineService = scriptEngineService;
            this.notificationService = notificationService;
            this.agentParseService = agentParseService;
            this.backgroundTaskQueue = backgroundTaskQueue;
        }

        /// <summary>
        /// <see cref="IAgentRunnerService.Running(AgentRun, List<Agent>, CancellationToken)"/>
        /// </summary>
        public List<string> Running(AgentRun agentRun, List<Agent> agents, CancellationToken cancellationToken = default)
        {
            var agentsRunning = new List<string>();

            if (this.backgroundTaskQueue.AgentRunCount != 0)
            {
                var keys = this.backgroundTaskQueue.AgentRunKeys;
                foreach (var agent in agents)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    agentRun.Agent = agent;
                    var key = this.GetKey(agentRun);

                    if (keys.Any(c => c.Contains(key)))
                    {
                        agentsRunning.Add(agent.Name);
                    }
                }
            }

            return agentsRunning;
        }

        /// <summary>
        /// <see cref="IAgentRunnerService.RunAsync(AgentRun, CancellationToken)"></see>
        /// </summary>
        public async Task RunAsync(AgentRun agentRun, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.backgroundTaskQueue.InitializeCurrentAgentRun();

            var channel = this.GetChannel(agentRun);

            Thread.Sleep(1000);

            if (!this.NeedToRunInEachSubdomain(agentRun.Agent, agentRun.Subdomain))
            {
                await this.RunBashAsync(agentRun, channel, last: true);
                return;
            }

            var subdomains = agentRun.RootDomain.Subdomains.ToList();
            var subdomainsCount = subdomains.Count;
            foreach (var subdomain in subdomains)
            {
                var last = subdomainsCount == 1;
                await this.RunBashAsync(new AgentRun
                {
                    Agent = agentRun.Agent,
                    Target = agentRun.Target,
                    RootDomain = agentRun.RootDomain,
                    Subdomain = subdomain,
                    ActivateNotification = agentRun.ActivateNotification,
                    Command = agentRun.Command
                }, channel, last);

                subdomainsCount--;
            }

            if (subdomains.Count == 0)
            {
                await this.SendAgentDoneNotificationAsync(agentRun, channel, cancellationToken);
            }
        }

        /// <summary>
        /// <see cref="IAgentRunnerService.StopAsync(AgentRun, bool, CancellationToken)"></see>
        /// </summary>
        public async Task StopAsync(AgentRun agentRun, bool removeSubdomainForTheKey, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            agentRun.Subdomain = removeSubdomainForTheKey ? null : agentRun.Subdomain;
            var channel = this.GetChannel(agentRun);

            try
            {   
                var key = this.GetKey(agentRun);
                await this.backgroundTaskQueue.StopCurrentAgentRunAsync(key);
            }
            catch (Exception ex)
            {
                await this.connectorService.SendAsync(channel, ex.Message, cancellationToken);
            }
            finally
            {
                await this.SendAgentDoneNotificationAsync(agentRun, channel, cancellationToken);
            }
        }

        /// <summary>
        /// Method to run a bash command
        /// </summary>
        /// <param name="agentRun"></param>
        /// <param name="channel">The channel to send the menssage</param>
        /// <param name="last"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>A Task</returns>
        private Task RunBashAsync(AgentRun agentRun, string channel, bool last)
        {
            if (this.backgroundTaskQueue.IsCurrentAgentRunStopped())
            {
                return Task.CompletedTask;
            }

            var runnerProcess = new RunnerProcess();
            this.backgroundTaskQueue.QueueAgentRun(new AgentRunProcess(this.GetKey(agentRun), runnerProcess, async token =>
            {
                var time = DateTime.Now.ToString("hh:mm:ss tt");

                try
                {                    
                    var needToSkip = this.NeedToSkipSubdomain(agentRun);
                    if (needToSkip)
                    {
                        await this.SendMsgLogAsync(channel, $"[{time}] Skip subdomain: {agentRun.Subdomain.Name}", token);
                        await this.SendMsgAsync(channel, $"[{time}] Skip subdomain: {agentRun.Subdomain.Name} ", token);
                    }
                    else
                    {
                        var command = this.GetCommand(agentRun);
                        runnerProcess.Start(command);

                        await this.SendMsgLogAsync(channel, $"[{time}] RUN: {command}", token);
                        await this.SendMsgAsync(channel, $"[{time}] RUN: {command}", token);

                        this.scriptEngineService.InintializeAgent(agentRun.Agent);

                        int lineCount = 1;
                        while (!runnerProcess.EndOfStream)
                        {
                            token.ThrowIfCancellationRequested();

                            var terminalLineOutput = runnerProcess.TerminalLineOutput();
                            var scriptOutput = await this.scriptEngineService.ParseInputAsync(terminalLineOutput, lineCount++);

                            await this.SendMsgLogHeadAsync(channel, lineCount, terminalLineOutput, scriptOutput, token);

                            await this.agentParseService.SaveScriptOutputAsync(agentRun, scriptOutput, token);

                            await this.SendMsgLogTailAsync(channel, lineCount, token);

                            await this.SendMsgAsync(channel, terminalLineOutput, token);
                        }

                        if (agentRun.Subdomain != null)
                        {
                            await this.agentParseService.RegisterAgentAsync(agentRun, token);
                        }
                    }
                }
                catch (Exception ex)
                {
                    await this.SendMsgAsync(channel, ex.Message, token);
                    await this.SendMsgLogAsync(channel, $"[{time}] Exception: {ex.StackTrace}", token);
                }
                finally
                {
                    if (last)
                    {
                        try
                        {
                            await this.StopAsync(agentRun, true, token);
                            await this.agentParseService.UpdateLastRunAsync(agentRun.Agent, token);
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
            }));

            return Task.CompletedTask;
        }

        /// <summary>
        /// Check if we need to skip the subdomain and does not the agent in that subdomain
        /// </summary>
        /// <param name="agentRun"></param>
        private bool NeedToSkipSubdomain(AgentRun agentRun)
        {
            var needToBeAlive = agentRun.Agent.OnlyIfIsAlive && (agentRun.Subdomain.IsAlive == null || !agentRun.Subdomain.IsAlive.Value);
            var needTohasHttpOpen = agentRun.Agent.OnlyIfHasHttpOpen && (agentRun.Subdomain.HasHttpOpen == null || !agentRun.Subdomain.HasHttpOpen.Value);
            var needToSkip = agentRun.Agent.SkipIfRanBefore && (!string.IsNullOrEmpty(agentRun.Subdomain.FromAgents) && agentRun.Subdomain.FromAgents.Contains(agentRun.Agent.Name));

            return needToBeAlive || needTohasHttpOpen || needToSkip;
        }

        /// <summary>
        /// Obtain the channel to send the menssage
        /// </summary>
        /// <param name="agent">The agent</param>
        /// <param name="rootDomain">The domain</param>
        /// <param name="subdomain">The subdomain</param>
        /// <returns>The channel to send the menssage</returns>
        private string GetChannel(AgentRun agentRun)
        {
            return agentRun.Subdomain == null ? 
                $"{agentRun.Agent.Name}_{agentRun.Target.Name}_{agentRun.RootDomain.Name}" : 
                $"{agentRun.Agent.Name}_{agentRun.Target.Name}_{agentRun.RootDomain.Name}_{agentRun.Subdomain.Name}";
        }

        /// <summary>
        /// Obtain the key to store the process
        /// </summary>
        /// <param name="agent">The agent</param>
        /// <param name="rootDomain">The domain</param>
        /// <param name="subdomain">The subdomain</param>
        /// <returns>The channel to send the menssage</returns>
        private string GetKey(AgentRun agentRun)
        {
            return agentRun.Subdomain == null ? 
                $"{agentRun.Agent.Name}_{agentRun.Target.Name}_{agentRun.RootDomain.Name}" : 
                $"{agentRun.Agent.Name}_{agentRun.Target.Name}_{agentRun.RootDomain.Name}_{agentRun.Subdomain.Name}";
        }

        /// <summary>
        /// Obtain if we need to run this agent in each target subdomains base on if the subdomain
        /// param if null and the agent can run in the subdomain level
        /// </summary>
        /// <param name="agent">The agent</param>
        /// <param name="subdomain">The subdomain</param>
        /// <returns>If we need to run this agent in each target subdomains</returns>
        private bool NeedToRunInEachSubdomain(Agent agent, Subdomain subdomain)
        {
            return subdomain == null && agent.IsBySubdomain;
        }

        /// <summary>
        /// Obtain the command to run on bash
        /// </summary>
        /// <param name="agentRun">The agent</param>
        /// <returns>The command to run on bash</returns>
        private string GetCommand(AgentRun agentRun)
        {
            var command = agentRun.Command;
            if (string.IsNullOrWhiteSpace(command))
            {
                command = agentRun.Agent.Command;
            }

            var envUserName = Environment.GetEnvironmentVariable("ReconnessUserName") ??
                              Environment.GetEnvironmentVariable("ReconnessUserName", EnvironmentVariableTarget.User);

            var envPassword = Environment.GetEnvironmentVariable("ReconnessPassword") ??
                              Environment.GetEnvironmentVariable("ReconnessPassword", EnvironmentVariableTarget.User);

            return $"{command.Replace("{{domain}}", agentRun.Subdomain == null ? agentRun.RootDomain.Name : agentRun.Subdomain.Name)}"
                .Replace("{{target}}", agentRun.Target.Name)
                .Replace("{{rootDomain}}", agentRun.RootDomain.Name)
                .Replace("{{userName}}", envUserName)
                .Replace("{{password}}", envPassword)
                .Replace("\"", "\\\"");
        }

        /// <summary>
        /// Send a msg and a notification when the agent finish
        /// </summary>
        /// <param name="agent">The agent</param>
        /// <param name="channel">The channel to use to send the msg</param>
        /// <param name="activateNotification">If we need to send a notification</param> 
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task SendAgentDoneNotificationAsync(AgentRun agentRun, string channel, CancellationToken cancellationToken)
        {
            if (agentRun.ActivateNotification && agentRun.Agent.NotifyIfAgentDone)
            {
                await this.notificationService.SendAsync($"Agent {agentRun.Agent.Name} is done!", cancellationToken);
            }

            await this.SendMsgAsync(channel, "Agent done!", cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="lineCount"></param>
        /// <param name="terminalLineOutput"></param>
        /// <param name="scriptOutput"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task SendMsgLogHeadAsync(string channel, int lineCount, string terminalLineOutput, ScriptOutput scriptOutput, CancellationToken cancellationToken)
        {
            await this.SendMsgLogAsync(channel, $"Output #: {lineCount}", cancellationToken);
            await this.SendMsgLogAsync(channel, $"Output: {terminalLineOutput}", cancellationToken);
            await this.SendMsgLogAsync(channel, $"Result: {JsonConvert.SerializeObject(scriptOutput)}", cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="lineCount"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task SendMsgLogTailAsync(string channel, int lineCount, CancellationToken cancellationToken)
        {
            await this.SendMsgLogAsync(channel, $"Output #: {lineCount} processed", cancellationToken);
            await this.SendMsgLogAsync(channel, "-----------------------------------------------------", cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="msg"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task SendMsgLogAsync(string channel, string msg, CancellationToken cancellationToken)
        {
            await this.connectorService.SendAsync("logs_" + channel, msg, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="msg"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task SendMsgAsync(string channel, string msg, CancellationToken cancellationToken)
        {
            await this.connectorService.SendAsync(channel, msg, cancellationToken);
        }
    }
}
