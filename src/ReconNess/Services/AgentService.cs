using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ReconNess.Core;
using ReconNess.Core.Models;
using ReconNess.Core.Services;
using ReconNess.Entities;
using RestSharp;

namespace ReconNess.Services
{
    /// <summary>
    /// This class implement <see cref="IAgentService"/>
    /// </summary>
    public class AgentService : Service<Agent>, IService<Agent>, IAgentService
    {
        private readonly ITargetService targetService;
        private readonly IConnectorService connectorService;
        private readonly IScriptEngineService scriptEngineService;
        private readonly IRunnerProcess runnerProcess;

        /// <summary>
        /// Initializes a new instance of the <see cref="AgentService" /> class
        /// </summary>
        /// <param name="unitOfWork"><see cref="IUnitOfWork"/></param>
        /// <param name="targetService"><see cref="ITargetService"/></param>
        /// <param name="connectorService"><see cref="IConnectorService"/></param>
        /// <param name="scriptEngineService"><see cref="IScriptEngineService"/></param>
        public AgentService(IUnitOfWork unitOfWork,
            ITargetService targetService,
            IConnectorService connectorService,
            IScriptEngineService scriptEngineService,
            IRunnerProcess runnerProcess)
            : base(unitOfWork)
        {
            this.targetService = targetService;
            this.connectorService = connectorService;
            this.scriptEngineService = scriptEngineService;
            this.runnerProcess = runnerProcess;
        }

        /// <summary>
        /// <see cref="IAgentService.GetAllAgentsWithCategoryAsync(CancellationToken)"/>
        /// </summary>
        public async Task<List<Agent>> GetAllAgentsWithCategoryAsync(CancellationToken cancellationToken = default)
        {
            return await this.GetAllQueryable(cancellationToken)
                .Include(a => a.AgentCategories)
                .ThenInclude(c => c.Category)
                .ToListAsync();
        }

        /// <summary>
        /// <see cref="IAgentService.GetAgentWithCategoryAsync(Expression{Func{Agent, bool}}, CancellationToken)"/>
        /// </summary>
        public async Task<Agent> GetAgentWithCategoryAsync(Expression<Func<Agent, bool>> criteria, CancellationToken cancellationToken = default)
        {
            return await this.GetAllQueryableByCriteria(criteria, cancellationToken)
                .Include(a => a.AgentCategories)
                .ThenInclude(c => c.Category)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// <see cref="IAgentService.GetDefaultAgentsToInstallAsync(CancellationToken)"/>
        /// </summary>
        public async Task<List<AgentDefault>> GetDefaultAgentsToInstallAsync(CancellationToken cancellationToken = default)
        {
            var client = new RestClient("https://raw.githubusercontent.com/");
            var request = new RestRequest("/reconness/reconness-agents/master/default-agents.json");

            var response = await client.ExecuteGetAsync(request, cancellationToken);
            var defaultAgents = JsonConvert.DeserializeObject<AgentDefaultList>(response.Content);

            return defaultAgents.Agents;
        }

        /// <summary>
        /// <see cref="IAgentService.GetAgentScript(string, CancellationToken)"/>
        /// </summary>
        public async Task<string> GetAgentScript(string scriptUrl, CancellationToken cancellationToken)
        {
            var client = new RestClient(scriptUrl);
            var request = new RestRequest();

            var response = await client.ExecuteGetAsync(request, cancellationToken);

            return response.Content;
        }

        /// <summary>
        /// <see cref="IAgentService.RunAsync(Target, Subdomain, Agent, string, CancellationToken)"></see>
        /// </summary>
        public async Task RunAsync(Target target, Subdomain subdomain, Agent agent, string command, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var channel = this.GetChannel(target, subdomain, agent);

            this.runnerProcess.Stopped = false;

            if (this.NeedToRunInEachSubdomain(subdomain, agent))
            {
                // wait 1 sec to avoid broke the frontend modal
                Thread.Sleep(1000);

                foreach (var sub in target.Subdomains.ToList())
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        this.runnerProcess.Stopped = true;
                        this.runnerProcess.KillProcess();

                        cancellationToken.ThrowIfCancellationRequested();
                    }

                    if (this.runnerProcess.Stopped)
                    {
                        break;
                    }

                    var needToBeAlive = agent.OnlyIfIsAlive && (sub.IsAlive == null || !sub.IsAlive.Value);
                    var needTohasHttpOpen = agent.OnlyIfHasHttpOpen && (sub.HasHttpOpen == null || !sub.HasHttpOpen.Value);
                    var needToSkip = agent.SkipIfRanBefore && (!string.IsNullOrEmpty(sub.FromAgents) && sub.FromAgents.Contains(agent.Name));
                    if (needToBeAlive || needTohasHttpOpen || needToSkip)
                    {
                        await this.connectorService.SendAsync("logs_" + channel, $"Skip subdomain: {sub.Name}");
                        continue;
                    }

                    var commandToRun = this.GetCommand(target, sub, agent, command);

                    await this.connectorService.SendAsync("logs_" + channel, $"RUN: {command}");
                    await this.RunBashAsync(target, sub, agent, commandToRun, channel, cancellationToken);
                }

                await this.connectorService.SendAsync(channel, "Agent done!", cancellationToken);
            }
            else
            {
                var commandToRun = this.GetCommand(target, subdomain, agent, command);

                await this.connectorService.SendAsync("logs_" + channel, $"RUN: {command}");
                await this.RunBashAsync(target, subdomain, agent, commandToRun, channel, cancellationToken);

                await this.connectorService.SendAsync(channel, "Agent done!");
            }

            agent.LastRun = DateTime.Now;
            await this.UpdateAsync(agent, cancellationToken);
        }

        /// <summary>
        /// <see cref="IAgentService.StopAsync(Target, Subdomain, Agent, CancellationToken)"></see>
        /// </summary>
        public async Task StopAsync(Target target, Subdomain subdomain, Agent agent, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var channel = subdomain == null ? $"{target.Name}_{agent.Name}" : $"{target.Name}_{subdomain.Name}_{agent.Name}";

            if (this.runnerProcess.IsRunning())
            {

                try
                {
                    this.runnerProcess.KillProcess();
                }
                catch (Exception ex)
                {
                    await this.connectorService.SendAsync(channel, ex.Message, cancellationToken);
                }
            }

            this.runnerProcess.Stopped = true;
            await this.connectorService.SendAsync(channel, "Agent stopped!", cancellationToken);
        }

        /// <summary>
        /// <see cref="IAgentService.DebugAsync(string, string, CancellationToken)"/>
        /// </summary>
        public async Task<ScriptOutput> DebugAsync(string terminalOutput, string script, CancellationToken cancellationToken = default)
        {
            return await this.scriptEngineService.ParseInputAsync(terminalOutput, 0, script);
        }

        /// <summary>
        /// Method to run a bash command
        /// </summary>
        /// <param name="channel">The channel to send the menssage</param>
        /// <param name="command">The command to run on bash</param>
        /// <returns>A Task</returns>
        private async Task RunBashAsync(Target target, Subdomain subdomain, Agent agent, string command, string channel, CancellationToken cancellationToken)
        {
            try
            {
                this.runnerProcess.StartProcess(command);
                this.scriptEngineService.InintializeAgent(agent);

                int lineCount = 1;
                while (this.runnerProcess.IsRunning() && !this.runnerProcess.EndOfStream)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var terminalLineOutput = this.runnerProcess.TerminalLineOutput();
                    var scriptOutput = await this.scriptEngineService.ParseInputAsync(terminalLineOutput, lineCount++);

                    await this.connectorService.SendAsync("logs_" + channel, $"Output #: {lineCount}");
                    await this.connectorService.SendAsync("logs_" + channel, $"Output: {terminalLineOutput}");
                    await this.connectorService.SendAsync("logs_" + channel, $"Result: {JsonConvert.SerializeObject(scriptOutput)}");

                    await this.targetService.SaveScriptOutputAsync(target, subdomain, agent, scriptOutput, cancellationToken);

                    await this.connectorService.SendAsync("logs_" + channel, $"Output #: {lineCount} processed");
                    await this.connectorService.SendAsync("logs_" + channel, "-----------------------------------------------------");

                    await this.connectorService.SendAsync(channel, terminalLineOutput, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                await SendLogException(channel, ex);
            }
            finally
            {
                this.runnerProcess.KillProcess();
            }
        }

        /// <summary>
        /// Obtain the channel to send the menssage
        /// </summary>
        /// <param name="target">The target</param>
        /// <param name="subdomain">The subdomain</param>
        /// <param name="agent">The agent</param>
        /// <returns>The channel to send the menssage</returns>
        private string GetChannel(Target target, Subdomain subdomain, Agent agent)
        {
            return subdomain == null ? $"{target.Name}_{agent.Name}" : $"{target.Name}_{subdomain.Name}_{agent.Name}";
        }

        /// <summary>
        /// Obtain if we need to run this agent in each target subdomains base on if the subdomain
        /// param if null and the agent can run in the subdomain level
        /// </summary>
        /// <param name="subdomain">The subdomain</param>
        /// <param name="agent">The agent</param>
        /// <returns>If we need to run this agent in each target subdomains</returns>
        private bool NeedToRunInEachSubdomain(Subdomain subdomain, Agent agent)
        {
            return subdomain == null && agent.IsBySubdomain;
        }

        /// <summary>
        /// Obtain the command to run on bash
        /// </summary>
        /// <param name="target">The target</param>
        /// <param name="subdomain">The subdomain</param>
        /// <param name="agent">The agent</param>
        /// <returns>The command to run on bash</returns>
        private string GetCommand(Target target, Subdomain subdomain, Agent agent, string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                command = agent.Command;
            }

            return $"{command.Replace("{{domain}}", subdomain == null ? target.RootDomain : subdomain.Name)}"
                .Replace("{{targetName}}", target.Name)
                .Replace("\"", "\\\"");
        }

        /// <summary>
        /// Send a log message
        /// </summary>
        /// <param name="channel">The channel logs to send the menssage</param>
        /// <param name="ex">The Exception Object</param>
        /// <returns>Send a log message</returns>
        private async Task SendLogException(string channel, Exception ex)
        {
            await this.connectorService.SendAsync(channel, ex.Message);
            await this.connectorService.SendAsync("logs_" + channel, $"Exception: {ex.StackTrace}");
        }        
    }
}
