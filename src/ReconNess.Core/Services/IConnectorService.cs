using ReconNess.Core.Models;
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
        /// <param name="includeTime">Include Time</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A Task</returns>
        Task SendAsync(string method, string msg, bool includeTime = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="lineCount"></param>
        /// <param name="terminalLineOutput"></param>
        /// <param name="scriptOutput"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task SendLogsHeadAsync(string channel, int lineCount, string terminalLineOutput, ScriptOutput scriptOutput, CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="lineCount"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task SendLogsTailAsync(string channel, int lineCount, CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="msg"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task SendLogsAsync(string channel, string msg, CancellationToken cancellationToken = default);
    }
}
