using ReconNess.Core;
using ReconNess.Core.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Worker
{
    /// <summary>
    /// 
    /// </summary>
    public class AgentRunBackgroundTaskQueue : IAgentRunBackgroundTaskQueue
    {
        private ConcurrentQueue<AgentRunProcess> workItems = new ConcurrentQueue<AgentRunProcess>();
        private SemaphoreSlim signal = new SemaphoreSlim(0);
        private AgentRunProcess currentRunProcess;

        private string keyDeleted;

        /// <summary>
        /// 
        /// </summary>
        public IList<string> AgentRunKeys
        {
            get
            {
                var keys = workItems.Select(a => a.Key).ToList();
                if (this.currentRunProcess != null)
                {
                    keys.Add(this.currentRunProcess.Key);
                }

                return keys;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int AgentRunCount
        {
            get
            {
                var count = this.workItems.Count();
                if (this.currentRunProcess != null)
                {
                    count++;
                }

                return count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workItem"></param>
        public void QueueAgentRun(AgentRunProcess workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            this.workItems.Enqueue(workItem);

            this.signal.Release();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<AgentRunProcess> DequeueAgentRunAsync(CancellationToken cancellationToken)
        {
            await this.signal.WaitAsync(cancellationToken);

            AgentRunProcess workItem;
            do
            {
                if (!this.workItems.TryDequeue(out workItem))
                {
                    return null;
                }

            } while (!string.IsNullOrEmpty(this.keyDeleted) && workItem.Key.Contains(this.keyDeleted));

            this.currentRunProcess = workItem;

            return workItem;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task StopCurrentAgentRunAsync(string key)
        {
            this.keyDeleted = key;
            if (this.currentRunProcess != null && this.currentRunProcess.Key.Contains(key))
            {
                if (this.currentRunProcess.RunnerProcess != null)
                {
                    this.currentRunProcess.RunnerProcess.KillProcess();
                    this.currentRunProcess = null;
                }
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        public void InitializeCurrentAgentRun()
        {
            this.keyDeleted = string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsCurrentAgentRunStopped()
        {
            return !string.IsNullOrEmpty(this.keyDeleted);
        }
    }
}
