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
        protected static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// <see cref="ILogsProvider.CleanLogfile(string, CancellationToken)"/>
        /// </summary>
        public void CleanLogfile(string logFileSelected, CancellationToken cancellationToken)
        {
            var bin = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            var path = Path.Combine(bin, "logs", logFileSelected).Substring(5);

            File.WriteAllText(path, string.Empty);
        }

        /// <summary>
        /// <see cref="ILogsProvider.GetLogfiles(CancellationToken)"/>
        /// </summary>
        public IEnumerable<string> GetLogfiles(CancellationToken cancellationToken)
        {
            var bin = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            var path = Path.Combine(bin, "logs").Substring(5);

            var files = Directory.GetFiles(path);

            return files.Select(f => Path.GetFileName(f));
        }

        /// <summary>
        /// <see cref="ILogsProvider.ReadLogfile(string, CancellationToken)"/>
        /// </summary>
        public async Task<string> ReadLogfileAsync(string logFileSelected, CancellationToken cancellationToken)
        {
            var bin = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            var path = Path.Combine(bin, "logs", logFileSelected).Substring(5);

            using (FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (BufferedStream bs = new BufferedStream(fs))
            using (StreamReader sr = new StreamReader(bs))
            {
                return await sr.ReadToEndAsync();
            }
        }
    }
}
