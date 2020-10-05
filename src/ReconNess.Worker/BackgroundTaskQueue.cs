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

        private string channelDeleted;

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

            } while (workItem.Channel.Equals(this.channelDeleted));

            this.currentRunProcess = workItem;

            return workItem;
        }

        /// <summary>
        /// <see cref="IBackgroundTaskQueue.RunningChannels"></see>
        /// </summary>
        public IList<string> RunningChannels
        {
            get
            {
                var channels = workItems.Select(a => a.Channel).Distinct().ToList();
                if (this.currentRunProcess != null)
                {
                    if (!channels.Any(c => c.Equals(this.currentRunProcess.Channel)))
                    {
                        channels.Add(this.currentRunProcess.Channel);
                    }
                }

                return channels;
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
        public Task StopAsync(string channel)
        {
            this.channelDeleted = channel;
            if (this.currentRunProcess != null && this.currentRunProcess.Channel.Equals(channel))
            {
                if (this.currentRunProcess.ProcessWrapper != null)
                {
                    this.currentRunProcess.ProcessWrapper.KillProcess();
                    this.currentRunProcess = null;
                }
            }

            this.workItems.Clear();

            return Task.CompletedTask;
        }

        /// <summary>
        /// <see cref="IBackgroundTaskQueue.IsStopped(string)"></see>
        /// </summary>
        public bool IsStopped(string channelDeleted)
        {
            return channelDeleted.Equals(this.channelDeleted);
        }

        /// <summary>
        /// <see cref="IBackgroundTaskQueue.Initializes(string)"></see>
        /// </summary>
        public void Initializes(string channel)
        {
            this.channelDeleted = string.Empty;
        }
    }
}
