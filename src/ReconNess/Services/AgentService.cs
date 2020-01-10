using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private static Process process;

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
            IScriptEngineService scriptEngineService)
            : base(unitOfWork)
        {
            this.targetService = targetService;
            this.connectorService = connectorService;
            this.scriptEngineService = scriptEngineService;
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
        /// <see cref="IAgentService.GetAllAgentsWithCategoryAsync(bool,CancellationToken)"/>
        /// </summary>
        public async Task<List<Agent>> GetAllAgentsWithCategoryAsync(bool isBySubdomain, CancellationToken cancellationToken = default)
        {
            var query = isBySubdomain ?
                this.GetAllQueryableByCriteria(a => a.IsBySubdomain, cancellationToken) :
                this.GetAllQueryable(cancellationToken);

            return await query
                .Include(a => a.AgentCategories)
                .ThenInclude(c => c.Category)
                .ToListAsync();
        }

        /// <summary>
        /// <see cref="IAgentService.RunAsync(Target, Subdomain, Agent, string, CancellationToken)"></see>
        /// </summary>
        public async Task RunAsync(Target target, Subdomain subdomain, Agent agent, string arguments, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(arguments))
            {
                arguments = agent.Arguments;
            }

            var channel = this.GetChannel(target, subdomain, agent);
            if (this.NeedToRunInEachSubdomain(subdomain, agent))
            {
                // wait 1 sec to avoid broke the frontend modal
                Thread.Sleep(1000);

                foreach (var sub in target.Subdomains.ToList())
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var needToBeAlive = agent.OnlyIfIsAlive && (sub.IsAlive == null || !sub.IsAlive.Value);
                    var needToSkip = agent.SkipIfRanBefore && (!string.IsNullOrEmpty(sub.FromAgents) && sub.FromAgents.Contains(agent.Name));
                    if (needToBeAlive || needToSkip)
                    {
                        await this.connectorService.SendAsync("Logs_" + channel, $"Skip subdomain: {sub.Name}");
                        continue;
                    }

                    var command = this.GetCommand(target, sub, agent, arguments);

                    await this.RunBashAsync(target, sub, agent, command, channel, cancellationToken);
                }

                await this.connectorService.SendAsync(channel, "Agent done!", cancellationToken);
            }
            else
            {
                var command = this.GetCommand(target, subdomain, agent, arguments);

                await this.RunBashAsync(target, subdomain, agent, command, channel, cancellationToken);

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

            if (process != null)
            {
                var channel = subdomain == null ? $"{target.Name}_{agent.Name}" : $"{target.Name}_{subdomain.Name}_{agent.Name}";
                try
                {
                    process.Kill();
                    process.WaitForExit();
                    process = null;
                }
                catch (Exception ex)
                {
                    await this.connectorService.SendAsync(channel, ex.Message, cancellationToken);
                }

                await this.connectorService.SendAsync(channel, "Agent stopped!", cancellationToken);
            }
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
                this.StartProcess(agent, command);

                int lineCount = 1;
                while (process != null && !process.StandardOutput.EndOfStream)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var terminalLineOutput = process.StandardOutput.ReadLine();
                    var scriptOutput = await this.scriptEngineService.ParseInputAsync(terminalLineOutput, lineCount++);

                    await this.connectorService.SendAsync("Logs_" + channel, $"Output #: {lineCount}");
                    await this.connectorService.SendAsync("Logs_" + channel, $"Output: {terminalLineOutput}");
                    await this.connectorService.SendAsync("Logs_" + channel, $"Result: {JsonConvert.SerializeObject(scriptOutput)}");

                    await this.targetService.SaveScriptOutputAsync(target, subdomain, agent, scriptOutput, cancellationToken);

                    await this.connectorService.SendAsync("Logs_" + channel, $"Output #: {lineCount} processed");
                    await this.connectorService.SendAsync("Logs_" + channel, "-----------------------------------------------------");

                    await this.connectorService.SendAsync(channel, terminalLineOutput, cancellationToken);
                }

                process.WaitForExit();
            }
            catch (Exception ex)
            {
                await SendLogException(channel, ex);
            }
            finally
            {
                process = null;
            }
        }

        /// <summary>
        /// Star the process
        /// </summary>
        /// <param name="agent">The agent</param>
        /// <param name="command">The command</param>
        private void StartProcess(Agent agent, string command)
        {
            process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{command}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            // wait 1 sec to avoid broke the frontend modal
            Thread.Sleep(1000);

            process.Start();

            this.scriptEngineService.InintializeAgent(agent);
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
        /// <param name="arguments"></param>
        /// <returns>The command to run on bash</returns>
        private string GetCommand(Target target, Subdomain subdomain, Agent agent, string arguments)
        {
            return $"{agent.Command} {arguments.Replace("{{domain}}", subdomain == null ? target.RootDomain : subdomain.Name)}".Replace("\"", "\\\""); ;
        }

        /// <summary>
        /// Send a log message
        /// </summary>
        /// <param name="channel">The channel logs to send the menssage</param>
        /// <param name="ex">The Exception Object</param>
        /// <returns>Send a log message</returns>
        private async Task SendLogException(string channel, Exception ex)
        {
            try
            {
                if (process != null)
                {
                    process.WaitForExit();
                }
            }
            catch (Exception) { }

            await this.connectorService.SendAsync(channel, ex.Message);
            await this.connectorService.SendAsync("Logs_" + channel, $"Exception: {ex.StackTrace}");
        }
    }
}
