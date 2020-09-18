using ReconNess.Worker.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Worker
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBackgroundTaskQueue
    {
        /// <summary>
        /// 
        /// </summary>
        int RunningCountAsync { get; }

        /// <summary>
        /// 
        /// </summary>
        IList<string> RunningKeysAsync { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="agentRunProcess"></param>
        void QueueAgentRun(AgentRunnerProcess agentRunProcess);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<AgentRunnerProcess> DequeueAgentRunAsync(CancellationToken cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task StopAsync(string key);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool IsStoppedAsync(string keyDeleted);
    }
}
