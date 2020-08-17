using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using ReconNess.Core;
using ReconNess.Core.Models;
using ReconNess.Core.Services;
using ReconNess.Entities;
using System;
using System.Collections.Concurrent;
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
        private readonly IBackgroundTaskQueue backgroundTaskQueue;

        /// <summary>
        /// Initializes a new instance of the <see cref="AgentRunnerService" /> class
        /// </summary>
        /// <param name="unitOfWork"><see cref="IUnitOfWork"/></param>
        /// <param name="connectorService"><see cref="IConnectorService"/></param>
        /// <param name="scriptEngineService"><see cref="IScriptEngineService"/></param>
        /// <param name="notificationService"><see cref="INotificationService"/></param>
        /// <param name="agentParseService"><see cref="IAgentParseService"/></param>
        /// <param name="backgroundTaskQueue"><see cref="IBackgroundTaskQueue"/></param>
        public AgentRunnerService(IUnitOfWork unitOfWork, 
            IConnectorService connectorService,
            IScriptEngineService scriptEngineService,            
            INotificationService notificationService,
            IAgentParseService agentParseService,
            IBackgroundTaskQueue backgroundTaskQueue) : base(unitOfWork)
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

            if (this.backgroundTaskQueue.Count != 0)
            {
                var keys = this.backgroundTaskQueue.Keys;
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

            this.backgroundTaskQueue.KeyDeleted = string.Empty;
            
            var channel = this.GetChannel(agentRun);

            if (!this.NeedToRunInEachSubdomain(agentRun.Agent, agentRun.Subdomain))
            {
                await this.RunBashAsync(agentRun, channel, true);
                return;
            }

            var subdomains = agentRun.RootDomain.Subdomains.ToList();
            var subdomainsCount = subdomains.Count;

            foreach (var subdomain in subdomains)
            {
                var needToSkip = this.NeedToSkipSubdomain(agentRun.Agent, subdomain);
                if (needToSkip)
                {
                    await this.SendMsgLogAsync(channel, $"Skip subdomain: {subdomain.Name}", cancellationToken);
                    await this.SendMsgAsync(channel, $"Skip subdomain: {subdomain.Name}", cancellationToken);
                    continue;
                }

                await this.RunBashAsync(new AgentRun
                {
                    Agent = agentRun.Agent,
                    Target = agentRun.Target,
                    RootDomain = agentRun.RootDomain,
                    Subdomain = subdomain,
                    ActivateNotification = agentRun.ActivateNotification,
                    Command = agentRun.Command
                }, channel, subdomainsCount == 1);

                subdomainsCount--;
            }            
        }

        /// <summary>
        /// <see cref="IAgentRunnerService.StopAsync(AgentRun, bool, CancellationToken)"></see>
        /// </summary>
        public async Task StopAsync(AgentRun agentRun, bool removeSubdomainForTheKey, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (removeSubdomainForTheKey)
            {
                agentRun.Subdomain = null;
            }

            var channel = this.GetChannel(agentRun);
            var key = this.GetKey(agentRun);

            try
            {
                this.backgroundTaskQueue.KeyDeleted = key;
                await this.backgroundTaskQueue.StopAndRemoveAsync(key);
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
            try
            {
                if (!string.IsNullOrEmpty(this.backgroundTaskQueue.KeyDeleted))
                {
                    return Task.CompletedTask;
                }

                var command = this.GetCommand(agentRun);  
                var runnerProcess = new RunnerProcess(command);

                this.backgroundTaskQueue.QueueBackgroundWorkItem(new AgentRunProcess(this.GetKey(agentRun), runnerProcess,async token =>
                {
                    try
                    {   
                        await this.SendMsgLogAsync(channel, $"RUN: {command}", token);
                        await this.SendMsgAsync(channel, $"RUN: {command}", token);
                        
                        this.scriptEngineService.InintializeAgent(agentRun.Agent);

                        int lineCount = 1;
                        while (!runnerProcess.EndOfStream)
                        {
                            token.ThrowIfCancellationRequested();

                            var terminalLineOutput = runnerProcess.TerminalLineOutput();
                            var scriptOutput = await this.scriptEngineService.ParseInputAsync(terminalLineOutput, lineCount++);

                            await SendMsgLogHeadAsync(channel, lineCount, terminalLineOutput, scriptOutput, token);
                            await this.agentParseService.SaveScriptOutputAsync(agentRun, scriptOutput, token);   
                            await SendMsgLogTailAsync(channel, lineCount, token);

                            await this.SendMsgAsync(channel, terminalLineOutput, token);
                        }
                    }
                    catch (Exception ex)
                    {
                        await this.SendMsgAsync(channel, ex.Message, token);
                        await this.SendMsgLogAsync(channel, $"Exception: {ex.StackTrace}", token);
                    }
                    finally
                    {
                        if (last)
                        {
                            try
                            {
                                await this.StopAsync(agentRun, true, token);
                                // update the last time that we run this agent
                                //agent.LastRun = DateTime.Now;
                                //await this.UpdateAsync(agent, cancellationToken);
                            }
                            catch(Exception)
                            {

                            }
                        }
                    }
                }));   
            }
            catch (Exception)
            {

            }         

            return Task.CompletedTask;
        }

        /// <summary>
        /// Check if we need to skip the subdomain and does not the agent in that subdomain
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="subdomain"></param>
        /// <returns></returns>
        private bool NeedToSkipSubdomain(Agent agent, Subdomain subdomain)
        {
            var needToBeAlive = agent.OnlyIfIsAlive && (subdomain.IsAlive == null || !subdomain.IsAlive.Value);
            var needTohasHttpOpen = agent.OnlyIfHasHttpOpen && (subdomain.HasHttpOpen == null || !subdomain.HasHttpOpen.Value);
            var needToSkip = agent.SkipIfRanBefore && (!string.IsNullOrEmpty(subdomain.FromAgents) && subdomain.FromAgents.Contains(agent.Name));

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
            return agentRun.Subdomain == null ? $"{agentRun.Agent.Name}_{agentRun.Target.Name}_{agentRun.RootDomain.Name}" : $"{agentRun.Agent.Name}_{agentRun.Target.Name}_{agentRun.RootDomain.Name}_{agentRun.Subdomain.Name}";
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
            return agentRun.Subdomain == null ? $"{agentRun.Agent.Name}_{agentRun.Target.Name}_{agentRun.RootDomain.Name}" : $"{agentRun.Agent.Name}_{agentRun.Target.Name}_{agentRun.RootDomain.Name}_{agentRun.Subdomain.Name}";
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

        private async Task SendMsgLogHeadAsync(string channel, int lineCount, string terminalLineOutput, ScriptOutput scriptOutput, CancellationToken cancellationToken)
        {
            await this.SendMsgLogAsync(channel, $"Output #: {lineCount}", cancellationToken);
            await this.SendMsgLogAsync(channel, $"Output: {terminalLineOutput}", cancellationToken);
            await this.SendMsgLogAsync(channel, $"Result: {JsonConvert.SerializeObject(scriptOutput)}", cancellationToken);
        }

        private async Task SendMsgLogTailAsync(string channel, int lineCount, CancellationToken cancellationToken)
        {
            await this.SendMsgLogAsync(channel, $"Output #: {lineCount} processed", cancellationToken);
            await this.SendMsgLogAsync(channel, "-----------------------------------------------------", cancellationToken);
        }               

        private async Task SendMsgLogAsync(string channel, string msg, CancellationToken cancellationToken)
        {
            await this.connectorService.SendAsync("logs_" + channel, msg, cancellationToken);
        }

        private async Task SendMsgAsync(string channel, string msg, CancellationToken cancellationToken)
        {
            await this.connectorService.SendAsync(channel, msg, cancellationToken);
        }
    }
}
