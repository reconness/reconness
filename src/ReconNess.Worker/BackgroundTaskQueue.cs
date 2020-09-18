﻿using ReconNess.Worker.Models;
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
        /// 
        /// </summary>
        /// <param name="workItem"></param>
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
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        public IList<string> RunningKeysAsync
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
        public int RunningCountAsync
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
        /// <param name="key"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsStoppedAsync(string keyDeleted)
        {
            return !string.IsNullOrEmpty(this.keyDeleted) && keyDeleted.Equals(this.keyDeleted);
        }
    }
}
