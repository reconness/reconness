using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Core.Services
{
    /// <summary>
    /// The interface IConnectorService
    /// </summary>
    public interface IConnectorService
    {
        /// <summary>
        /// Send a message using the channel
        /// </summary>
        /// <param name="method">The channel</param>
        /// <param name="msg">The message</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A Task</returns>
        Task SendAsync(string method, string msg, CancellationToken cancellationToken = default);
    }
}
