using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Core.Providers
{
    /// <summary>
    /// The interface ILogsProvider
    /// </summary>
    public interface ILogsProvider
    {
        /// <summary>
        /// Obtain the list of logs file
        /// </summary>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The list of logs file</returns>
        IEnumerable<string> GetLogfiles(CancellationToken cancellationToken);

        /// <summary>
        /// Obtain the log file content
        /// </summary>
        /// <param name="logFileSelected">The log file</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The log file content</returns>
        ValueTask<string> ReadLogfileAsync(string logFileSelected, CancellationToken cancellationToken);

        /// <summary>
        /// Clean the log file
        /// </summary>
        /// <param name="logFileSelected">The log file</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        ValueTask CleanLogfileAsync(string logFileSelected, CancellationToken cancellationToken);
    }
}
