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
        int RunningCount { get; }

        /// <summary>
        /// The list of agent channel running
        /// </summary>
        IList<string> RunningChannels { get; }

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
        /// Stop the Agent with that channel
        /// </summary>
        /// <param name="channel">The Channel</param>
        /// <returns>A Task</returns>
        Task StopAsync(string channel);

        /// <summary>
        /// If the Agent with that channel is not running 
        /// </summary>
        /// <param name="channel">The Channel</param>
        /// <returns>If the Agent with that channel is not running </returns>
        bool IsStopped(string channel);

        /// <summary>
        /// Initializes the Agent with that channel
        /// </summary>
        /// <param name="channel">The Channel</param>
        /// <returns>A Task</returns>
        void Initializes(string channel);
    }
}
