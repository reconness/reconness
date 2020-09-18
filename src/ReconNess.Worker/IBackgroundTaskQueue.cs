using ReconNess.Worker.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Worker
{
    /// <summary>
    /// The interface IBackgroundTaskQueue
    /// </summary>
    public interface IBackgroundTaskQueue
    {
        /// <summary>
        /// The amount of Agent running
        /// </summary>
        int RunningCountAsync { get; }

        /// <summary>
        /// The list of agent keys running
        /// </summary>
        IList<string> RunningKeysAsync { get; }

        /// <summary>
        /// Queue an Agent to Run
        /// </summary>
        /// <param name="agentRunProcess">The Agent to queue</param>
        void QueueAgentRun(AgentRunnerProcess agentRunProcess);

        /// <summary>
        /// Dequeue the Agent to run
        /// </summary>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The Agent to tun after dequeue</returns>
        Task<AgentRunnerProcess> DequeueAgentRunAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Stop the Agent with that key
        /// </summary>
        /// <param name="key">The Agent key</param>
        /// <returns>A Task</returns>
        Task StopAsync(string key);

        /// <summary>
        /// If the Agent with that key is not running 
        /// </summary>
        /// <param name="key">The Agent key</param>
        /// <returns>If the Agent with that key is not running </returns>
        bool IsStoppedAsync(string key);
    }
}
