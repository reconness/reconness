using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Core.Providers
{
    /// <summary>
    /// The interface IAgentRunnerProvider
    /// </summary>
    public interface IQueueProvider<T>
    {
        /// <summary>
        /// Run the Agent
        /// </summary>
        /// <param name="args">The Agent params</param>
        /// <param name="cancellationToken"></param>
        /// <returns>A task</returns>
        void Enqueue(T args);
    }
}