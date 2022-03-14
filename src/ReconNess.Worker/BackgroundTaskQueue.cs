using NLog;
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
        protected static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        private readonly ConcurrentQueue<AgentRunnerProcess> workItems = new ConcurrentQueue<AgentRunnerProcess>();
        private readonly SemaphoreSlim signal = new SemaphoreSlim(0);
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

            workItems.Enqueue(workItem);

            signal.Release();
        }

        /// <summary>
        /// <see cref="IBackgroundTaskQueue.DequeueAgentRunAsync(CancellationToken)"></see>
        /// </summary>
        public async Task<AgentRunnerProcess> DequeueAgentRunAsync(CancellationToken cancellationToken)
        {
            await signal.WaitAsync(cancellationToken);

            AgentRunnerProcess workItem;
            do
            {

                if (!workItems.TryDequeue(out workItem))
                {
                    return null;
                }

            } while (workItem.Channel.Equals(channelDeleted));

            currentRunProcess = workItem;

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
                if (currentRunProcess != null)
                {
                    if (!channels.Any(c => c.Equals(currentRunProcess.Channel)))
                    {
                        channels.Add(currentRunProcess.Channel);
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
                var count = workItems.Count();
                if (currentRunProcess != null)
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
            channelDeleted = channel;
            if (currentRunProcess != null && currentRunProcess.Channel.Equals(channel))
            {
                if (currentRunProcess.ProcessWrapper != null)
                {
                    currentRunProcess.ProcessWrapper.StopProcess();
                    currentRunProcess = null;
                }
            }

            workItems.Clear();

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
            channelDeleted = string.Empty;
        }
    }
}
