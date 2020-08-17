using ReconNess.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Core
{
    public interface IBackgroundTaskQueue
    {
        int Count { get; }
        
        IList<string> Keys { get; }

        void QueueBackgroundWorkItem(AgentRunProcess workItem);

        Task<AgentRunProcess> DequeueAsync(CancellationToken cancellationToken);

        Task StopAndRemoveAsync(string key);
    }
}
