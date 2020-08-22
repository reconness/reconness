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
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private ConcurrentQueue<AgentRunProcess> workItems = new ConcurrentQueue<AgentRunProcess>();
        private SemaphoreSlim signal = new SemaphoreSlim(0);
        private AgentRunProcess currentRunProcess;

        private string keyDeleted;

        public IList<string> Keys
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

        public int Count
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

        public void QueueBackgroundWorkItem(AgentRunProcess workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            this.workItems.Enqueue(workItem);

            this.signal.Release();
        }

        public async Task<AgentRunProcess> DequeueAsync(CancellationToken cancellationToken)
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

        public Task StopAndRemoveAsync(string key)
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

        public void ResetKeyToDelete()
        {
            this.keyDeleted = string.Empty;
        }
    }
}
