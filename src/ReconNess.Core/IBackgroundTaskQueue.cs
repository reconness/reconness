using ReconNess.Core.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Core
{
    public interface IBackgroundTaskQueue
    {
        string KeyDeleted { get; set; }

        int Count { get; }

        IList<string> Keys { get; }

        void QueueBackgroundWorkItem(AgentRunProcess workItem);

        Task<AgentRunProcess> DequeueAsync(CancellationToken cancellationToken);

        Task StopAndRemoveAsync(string key);
    }
}
