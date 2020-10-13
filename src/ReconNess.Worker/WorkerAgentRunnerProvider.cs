using NLog;
using ReconNess.Core.Models;
using ReconNess.Core.Providers;
using ReconNess.Core.Services;
using ReconNess.Worker.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Worker
{
    /// <summary>
    /// This class implement <see cref="IAgentRunnerProvider"/>
    /// </summary>
    public class WorkerAgentRunnerProvider : IAgentRunnerProvider
    {
        protected static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        private readonly IBackgroundTaskQueue backgroundTaskQueue;
        private readonly IScriptEngineService scriptEngineService;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkerAgentRunnerProvider" /> class
        /// </summary>
        /// <param name="backgroundTaskQueue"><see cref="IBackgroundTaskQueue"/></param>
        /// <param name="scriptEngineService"><see cref="IScriptEngineService"/></param>
        public WorkerAgentRunnerProvider(
            IBackgroundTaskQueue backgroundTaskQueue,
            IScriptEngineService scriptEngineService)
        {
            this.backgroundTaskQueue = backgroundTaskQueue;
            this.scriptEngineService = scriptEngineService;
        }

        /// <summary>
        /// <see cref="IAgentRunnerProvider.RunningCountAsync"/>
        /// </summary>
        public Task<int> RunningCountAsync => Task.FromResult(this.backgroundTaskQueue.RunningCount);

        /// <summary>
        /// <see cref="IAgentRunnerProvider.RunningChannelsAsync"/>
        /// </summary>
        public Task<IList<string>> RunningChannelsAsync => Task.FromResult(this.backgroundTaskQueue.RunningChannels);

        /// <summary>
        /// <see cref="IAgentRunnerProvider.IsStoppedAsync(string)"/>
        /// </summary>
        public Task<bool> IsStoppedAsync(string channel) => Task.FromResult(this.backgroundTaskQueue.IsStopped(channel));

        /// <summary>
        /// <see cref="IAgentRunnerProvider.InitializesAsync(string)"/>
        /// </summary>
        public Task InitializesAsync(string channel)
        {
            this.backgroundTaskQueue.Initializes(channel);

            return Task.CompletedTask;
        }

        /// <summary>
        /// <see cref="IAgentRunnerProvider.StopAsync(string)"/>
        /// </summary>
        public async Task StopAsync(string channel) => await this.backgroundTaskQueue.StopAsync(channel);

        /// <summary>
        /// <see cref="IAgentRunnerProvider.RunAsync(AgentRunnerProviderArgs)"/>
        /// </summary>
        public Task RunAsync(AgentRunnerProviderArgs providerArgs)
        {
            var processWrapper = new ProcessWrapper();
            this.backgroundTaskQueue.QueueAgentRun(new AgentRunnerProcess(providerArgs.Channel, processWrapper, async (CancellationToken token) =>
            {
                try
                {
                    if (!await providerArgs.SkipHandlerAsync(new AgentRunnerProviderResult
                    {
                        AgentRunner = providerArgs.AgentRunner,
                        Channel = providerArgs.Channel,
                        Command = providerArgs.Command,
                        AgentRunnerType = providerArgs.AgentRunnerType,
                        CancellationToken = token
                    }))
                    {
                        await providerArgs.BeginHandlerAsync(new AgentRunnerProviderResult
                        {
                            Channel = providerArgs.Channel,
                            Command = providerArgs.Command,
                            CancellationToken = token
                        });

                        processWrapper.Start(providerArgs.Command);

                        var lineCount = 1;
                        var script = providerArgs.AgentRunner.Agent.Script;

                        while (!processWrapper.EndOfStream)
                        {
                            if (processWrapper.Stopped)
                            {
                                break;
                            }

                            token.ThrowIfCancellationRequested();

                            // Parse the terminal output one line
                            var terminalLineOutput = processWrapper.TerminalLineOutput();
                            var scriptOutput = await this.scriptEngineService.TerminalOutputParseAsync(script, terminalLineOutput, lineCount++);

                            await providerArgs.ParserOutputHandlerAsync(new AgentRunnerProviderResult
                            {
                                AgentRunner = providerArgs.AgentRunner,
                                Channel = providerArgs.Channel,
                                ScriptOutput = scriptOutput,
                                LineCount = lineCount,
                                TerminalLineOutput = terminalLineOutput,
                                CancellationToken = token
                            });
                        }
                    };

                    await providerArgs.EndHandlerAsync(new AgentRunnerProviderResult
                    {
                        AgentRunner = providerArgs.AgentRunner,
                        Channel = providerArgs.Channel,
                        AgentRunnerType = providerArgs.AgentRunnerType,
                        Command = providerArgs.Command,
                        Last = providerArgs.Last,
                        CancellationToken = token
                    });
                }
                catch (Exception ex)
                {
                    await providerArgs.ExceptionHandlerAsync(new AgentRunnerProviderResult
                    {
                        Channel = providerArgs.Channel,
                        AgentRunner = providerArgs.AgentRunner,
                        Last = providerArgs.Last,
                        Command = providerArgs.Command,
                        Exception = ex,
                        CancellationToken = token
                    });
                }
            }));

            return Task.CompletedTask;
        }
    }
}
