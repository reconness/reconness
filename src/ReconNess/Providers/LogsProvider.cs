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
        /// <summary>
        /// <see cref="ILogsProvider.CleanLogfile(string, CancellationToken)"/>
        /// </summary>
        public void CleanLogfile(string logFileSelected, CancellationToken cancellationToken)
        {
            var bin = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            var path = Path.Combine(bin, "logs", logFileSelected).Substring(6);

            System.IO.File.WriteAllText(path, string.Empty);
        }

        /// <summary>
        /// <see cref="ILogsProvider.GetLogfiles(CancellationToken)"/>
        /// </summary>
        public IEnumerable<string> GetLogfiles(CancellationToken cancellationToken)
        {
            var bin = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            var path = Path.Combine(bin, "logs").Substring(6);

            var files = Directory.GetFiles(path);

            return files.Select(f => Path.GetFileName(f));
        }

        /// <summary>
        /// <see cref="ILogsProvider.ReadLogfile(string, CancellationToken)"/>
        /// </summary>
        public async Task<string> ReadLogfile(string logFileSelected, CancellationToken cancellationToken)
        {
            var bin = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            var path = Path.Combine(bin, "logs", logFileSelected).Substring(6);

            using (FileStream fs = System.IO.File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (BufferedStream bs = new BufferedStream(fs))
            using (StreamReader sr = new StreamReader(bs))
            {
                return await sr.ReadToEndAsync();
            }
        }
    }
}
