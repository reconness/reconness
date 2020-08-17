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

        public IList<string> Keys 
        {
            get
            {
                var keys = workItems.Where(w => !w.MarkAsDeleted).Select(a => a.Key).ToList();
                if (this.currentRunProcess != null && !this.currentRunProcess.MarkAsDeleted)
                {
                    keys.Add(this.currentRunProcess.Key);
                }

                return keys;
            }
        }

        public int Count => this.workItems.Count;

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

            AgentRunProcess workItem = null;
            do
            {
                this.workItems.TryDequeue(out workItem);

            } while (workItem.MarkAsDeleted);

            this.currentRunProcess = workItem;

            return workItem;
        }

        public Task StopAndRemoveAsync(string key)
        {
            workItems.ToList().ForEach(workItem =>
            {
                if (workItem.Key.Contains(key))
                {
                    workItem.MarkAsDeleted = true;
                }
            });

            if (this.currentRunProcess != null && !this.currentRunProcess.MarkAsDeleted && 
                this.currentRunProcess.Key.Contains(key) && this.currentRunProcess.RunnerProcess != null)
            {
                this.currentRunProcess.RunnerProcess.KillProcess();
                this.currentRunProcess.MarkAsDeleted = true;
            }

            return Task.CompletedTask;
        }
    }
}
