using Microsoft.Extensions.Hosting;
using NLog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Worker
{
    public class QueuedHostedService : BackgroundService
    {
        protected static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public QueuedHostedService(IBackgroundTaskQueue taskQueue)
        {
            TaskQueue = taskQueue;
        }

        public IBackgroundTaskQueue TaskQueue { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.Info(
                $"Queued Hosted Service is running.{Environment.NewLine}" +
                $"{Environment.NewLine}Tap W to add a work item to the " +
                $"background queue.{Environment.NewLine}");

            await BackgroundProcessing(stoppingToken);
        }

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                var workItem =
                    await TaskQueue.DequeueAgentRunAsync(stoppingToken);

                try
                {
                    if (workItem != null)
                    {
                        _logger.Info("WorkItem Started");
                        await workItem.ProcessFunc(stoppingToken);
                        _logger.Info("WorkItem finished");
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex,
                        $"Error occurred executing {workItem}.", nameof(workItem));
                }
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.Info("Queued Hosted Service is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}
