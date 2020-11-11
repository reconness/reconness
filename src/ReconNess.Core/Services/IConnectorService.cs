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
        /// <param name="channel">The channel</param>
        /// <param name="msg">The message</param>
        /// <param name="includeTime">Include Time</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A Task</returns>
        Task SendAsync(string channel, string msg, bool includeTime = true, CancellationToken cancellationToken = default);


        /// <summary>
        /// Send a message using the log channel
        /// </summary>
        /// <param name="channel">The channel</param>
        /// <param name="msg">The message</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A Task</returns>
        Task SendLogsAsync(string channel, string msg, CancellationToken cancellationToken = default);
    }
}
