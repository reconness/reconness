using ReconNess.Core;
using ReconNess.Core.Models;
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
        /// <see cref="IAgentRunnerProvider.RunningKeysAsync"/>
        /// </summary>
        public Task<IList<string>> RunningKeysAsync => Task.FromResult(this.backgroundTaskQueue.RunningKeys);

        /// <summary>
        /// <see cref="IAgentRunnerProvider.IsStoppedAsync(string)"/>
        /// </summary>
        public Task<bool> IsStoppedAsync(string key) => Task.FromResult(this.backgroundTaskQueue.IsStopped(key));

        /// <summary>
        /// <see cref="IAgentRunnerProvider.InitializesAsync(string)"/>
        /// </summary>
        public Task InitializesAsync(string key)
        {
            this.backgroundTaskQueue.Initializes(key);

            return Task.CompletedTask;
        }

        /// <summary>
        /// <see cref="IAgentRunnerProvider.StopAsync(string)"/>
        /// </summary>
        public async Task StopAsync(string key) => await this.backgroundTaskQueue.StopAsync(key);

        /// <summary>
        /// <see cref="IAgentRunnerProvider.RunAsync(AgentRunnerProviderArgs)"/>
        /// </summary>
        public Task RunAsync(AgentRunnerProviderArgs providerArgs)
        {
            var processWrapper = new ProcessWrapper();
            this.backgroundTaskQueue.QueueAgentRun(new AgentRunnerProcess(providerArgs.Key, processWrapper, async (CancellationToken token) =>
            {
                try
                {
                    if(!await providerArgs.SkipHandlerAsync(new AgentRunnerProviderResult
                    {
                        AgentRunner = providerArgs.AgentRunner,
                        AgentRunnerType = providerArgs.AgentRunnerType,
                        Channel = providerArgs.Channel,
                        Command = providerArgs.Command,
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
                        Key = providerArgs.Key,
                        AgentRunner = providerArgs.AgentRunner,
                        AgentRunnerType = providerArgs.AgentRunnerType,
                        Channel = providerArgs.Channel,
                        Last = providerArgs.Last,                        
                        CancellationToken = token
                    });

                }
                catch (Exception ex)
                {
                    await providerArgs.ExceptionHandlerAsync(new AgentRunnerProviderResult
                    {
                        Key = providerArgs.Key,
                        AgentRunner = providerArgs.AgentRunner,
                        Channel = providerArgs.Channel,
                        Last = providerArgs.Last,
                        Exception = ex,
                        CancellationToken = token
                    });
                }
            }));

            return Task.CompletedTask;
        }
    }
}
