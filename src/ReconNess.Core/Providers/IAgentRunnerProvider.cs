﻿using ReconNess.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReconNess.Core.Providers
{
    /// <summary>
    /// The interface IAgentRunnerProvider
    /// </summary>
    public interface IAgentRunnerProvider
    {
        /// <summary>
        /// A callback method when the Agent start running
        /// </summary>
        /// <param name="result">The result send to the callback method</param>
        /// <returns>A task</returns>
        public delegate Task BeginHandlerAsync(AgentRunnerProviderResult result);

        /// <summary>
        /// A callback method when the Agent to check if we can skip running
        /// </summary>
        /// <param name="result">The result send to the callback method</param>
        /// <returns>A task </returns>
        public delegate ValueTask<bool> SkipHandlerAsync(AgentRunnerProviderResult result);

        /// <summary>
        /// A callback method when the Agent parse the terminal output
        /// </summary>
        /// <param name="result">The result send to the callback method</param>
        /// <returns>A task</returns>
        public delegate Task ParserOutputHandlerAsync(AgentRunnerProviderResult result);

        /// <summary>
        /// A callback method when the Agent end running
        /// </summary>
        /// <param name="result">The result send to the callback method</param>
        /// <returns>A task</returns>
        public delegate Task EndHandlerAsync(AgentRunnerProviderResult result);

        /// <summary>
        /// A callback method when the Agent throw exception
        /// </summary>
        /// <param name="result">The result send to the callback method</param>
        /// <returns>A task</returns>
        public delegate Task ExceptionHandlerAsync(AgentRunnerProviderResult result);

        /// <summary>
        /// The amount of Agent running
        /// </summary>
        Task<int> RunningCountAsync { get; }

        /// <summary>
        /// The list of agent channels running
        /// </summary>
        Task<IList<string>> RunningChannelsAsync { get; }

        /// <summary>
        /// Initializes the Agent with that channel
        /// </summary>
        /// <param name="channel">The Channel</param>
        /// <returns>A task</returns>
        Task InitializesAsync(string channel);

        /// <summary>
        /// Run the Agent
        /// </summary>
        /// <param name="providerArgs">The Agent params</param>
        /// <returns>A task</returns>
        Task RunAsync(AgentRunnerProviderArgs providerArgs);

        /// <summary>
        /// Stop the Agent with that channel
        /// </summary>
        /// <param name="channel">The Channel</param>
        /// <returns>A task</returns>
        Task StopAsync(string channel);

        /// <summary>
        /// If the Agent with that channel is not running 
        /// </summary>
        /// <param name="channel">The Channel</param>
        /// <returns>If the Agent with that channel is not running </returns>
        Task<bool> IsStoppedAsync(string channel);
    }
}