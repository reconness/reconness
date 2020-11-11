using NLog;
using ReconNess.Core.Providers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Providers
{
    /// <summary>
    /// This class implement <see cref="ILogsProvider"/>
    /// </summary>
    public class LogsProvider : ILogsProvider
    {
        private const int INDEX = 5;

        protected static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// <see cref="ILogsProvider.GetLogfiles(CancellationToken)"/>
        /// </summary>
        public IEnumerable<string> GetLogfiles(CancellationToken cancellationToken)
        {
            var logPath = this.GetLogPath();

            var files = Directory.GetFiles(logPath);

            return files.Select(f => Path.GetFileName(f));
        }

        /// <summary>
        /// <see cref="ILogsProvider.ReadLogfile(string, CancellationToken)"/>
        /// </summary>
        public async Task<string> ReadLogfileAsync(string logFileSelected, CancellationToken cancellationToken)
        {
            var logPath = this.GetLogPath();

            string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            foreach (char c in invalid)
            {
                logFileSelected = logFileSelected.Replace(c.ToString(), "");
            }

            var path = Path.Combine(logPath, logFileSelected);
            if (path.StartsWith(logPath))
            {
                using (FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (BufferedStream bs = new BufferedStream(fs))
                using (StreamReader sr = new StreamReader(bs))
                {
                    return await sr.ReadToEndAsync();
                }
            }
            else
            {
                _logger.Warn($"Invalid Log file path {path}");
            }

            return string.Empty;
        }

        /// <summary>
        /// <see cref="ILogsProvider.CleanLogfileAsync(string, CancellationToken)"/>
        /// </summary>
        public async Task CleanLogfileAsync(string logFileSelected, CancellationToken cancellationToken)
        {
            var logPath = this.GetLogPath();

            string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            foreach (char c in invalid)
            {
                logFileSelected = logFileSelected.Replace(c.ToString(), "");
            }

            var path = Path.Combine(logPath, logFileSelected);
            if (path.StartsWith(logPath))
            {
                await File.WriteAllTextAsync(path, string.Empty);
            }
            else
            {
                _logger.Warn($"Invalid Log file path {path}");
            }
        }

        /// <summary>
        /// Obtain the log folder path
        /// </summary>
        /// <returns>The log folder path</returns>
        private string GetLogPath()
        {
            var bin = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location).Substring(INDEX);
            var path = Path.Combine(bin, "logs");

            return path;
        }
    }
}
