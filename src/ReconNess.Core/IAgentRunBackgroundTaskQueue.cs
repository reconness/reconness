using ReconNess.Core.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Core
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAgentRunBackgroundTaskQueue
    {
        /// <summary>
        /// 
        /// </summary>
        int AgentRunCount { get; }

        /// <summary>
        /// 
        /// </summary>
        IList<string> AgentRunKeys { get; }

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
        Task StopCurrentAgentRunAsync(string key);

        /// <summary>
        /// 
        /// </summary>
        void InitializeCurrentAgentRun();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool IsCurrentAgentRunStopped();
    }
}
