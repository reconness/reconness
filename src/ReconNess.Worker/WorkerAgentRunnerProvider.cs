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
    /// 
    /// </summary>
    public class WorkerAgentRunnerProvider : IAgentRunnerProvider
    {
        private readonly IBackgroundTaskQueue backgroundTaskQueue;
        private readonly IScriptEngineService scriptEngineService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="backgroundTaskQueue"></param>
        /// <param name="scriptEngineService"></param>
        public WorkerAgentRunnerProvider(
            IBackgroundTaskQueue backgroundTaskQueue,
            IScriptEngineService scriptEngineService)
        {
            this.backgroundTaskQueue = backgroundTaskQueue;
            this.scriptEngineService = scriptEngineService;
        }

        /// <summary>
        /// 
        /// </summary>
        public Task<int> RunningCountAsync => Task.FromResult(this.backgroundTaskQueue.RunningCountAsync);

        /// <summary>
        /// 
        /// </summary>
        public Task<IList<string>> RunningKeysAsync => Task.FromResult(this.backgroundTaskQueue.RunningKeysAsync);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<bool> IsStoppedAsync(string key) => Task.FromResult(this.backgroundTaskQueue.IsStoppedAsync(key));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task StopAsync(string key) => await this.backgroundTaskQueue.StopAsync(key);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="providerArgs"></param>
        /// <returns></returns>
        public Task RunAsync(AgentRunnerProviderArgs providerArgs)
        {
            var processWrapper = new ProcessWrapper();
            this.backgroundTaskQueue.QueueAgentRun(new AgentRunnerProcess(providerArgs.Key, processWrapper, async (CancellationToken token) =>
            {
                try
                {
                    await providerArgs.BeginHandlerAsync(new AgentRunnerProviderHandlerArgs
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

                        await providerArgs.ParserOutputHandlerAsync(new AgentRunnerProviderHandlerArgs
                        {
                            AgentRunner = providerArgs.AgentRunner,
                            Channel = providerArgs.Channel,
                            ScriptOutput = scriptOutput,
                            LineCount = lineCount,
                            TerminalLineOutput = terminalLineOutput,
                            CancellationToken = token
                        });
                    }

                    await providerArgs.EndHandlerAsync(new AgentRunnerProviderHandlerArgs
                    {
                        AgentRunner = providerArgs.AgentRunner,
                        Channel = providerArgs.Channel,
                        Last = providerArgs.Last,
                        RemoveSubdomainForTheKey = providerArgs.RemoveSubdomainForTheKey,
                        CancellationToken = token
                    });

                }
                catch (Exception ex)
                {
                    await providerArgs.ExceptionHandlerAsync(new AgentRunnerProviderHandlerArgs
                    {
                        AgentRunner = providerArgs.AgentRunner,
                        Channel = providerArgs.Channel,
                        Last = providerArgs.Last,
                        RemoveSubdomainForTheKey = providerArgs.RemoveSubdomainForTheKey,
                        Exception = ex,
                        CancellationToken = token
                    });
                }
            }));

            return Task.CompletedTask;
        }
    }
}
