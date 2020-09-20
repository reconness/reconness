using ReconNess.Worker.Models;
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
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private ConcurrentQueue<AgentRunnerProcess> workItems = new ConcurrentQueue<AgentRunnerProcess>();
        private SemaphoreSlim signal = new SemaphoreSlim(0);
        private AgentRunnerProcess currentRunProcess;

        private string keyDeleted;

        /// <summary>
        /// <see cref="IBackgroundTaskQueue.QueueAgentRun(AgentRunnerProcess)"></see>
        /// </summary>
        public void QueueAgentRun(AgentRunnerProcess workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            this.workItems.Enqueue(workItem);

            this.signal.Release();
        }

        /// <summary>
        /// <see cref="IBackgroundTaskQueue.DequeueAgentRunAsync(CancellationToken)"></see>
        /// </summary>
        public async Task<AgentRunnerProcess> DequeueAgentRunAsync(CancellationToken cancellationToken)
        {
            await this.signal.WaitAsync(cancellationToken);

            AgentRunnerProcess workItem;
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
        /// <see cref="IBackgroundTaskQueue.RunningKeys"></see>
        /// </summary>
        public IList<string> RunningKeys
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
        /// <see cref="IBackgroundTaskQueue.RunningCount"></see>
        /// </summary>
        public int RunningCount
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
        /// <see cref="IBackgroundTaskQueue.StopAsync(string)"></see>
        /// </summary>
        public Task StopAsync(string key)
        {
            this.keyDeleted = key;
            if (this.currentRunProcess != null && this.currentRunProcess.Key.Contains(key))
            {
                if (this.currentRunProcess.ProcessWrapper != null)
                {
                    this.currentRunProcess.ProcessWrapper.KillProcess();
                    this.currentRunProcess = null;
                }
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// <see cref="IBackgroundTaskQueue.IsStopped(string)"></see>
        /// </summary>
        public bool IsStopped(string keyDeleted)
        {
            return !string.IsNullOrEmpty(this.keyDeleted) && keyDeleted.Equals(this.keyDeleted);
        }

        /// <summary>
        /// <see cref="IBackgroundTaskQueue.Initializes(string)"></see>
        /// </summary>
        public void Initializes(string key)
        {
            this.keyDeleted = string.Empty;
        }
    }
}
