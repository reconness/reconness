using NLog;
using ReconNess.Application.Providers;

namespace ReconNess.Infrastructure.Providers;

/// <summary>
/// This class implement <see cref="ILogsProvider"/>
/// </summary>
public class LogsProvider : ILogsProvider
{
    protected static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    /// <inheritdoc/>
    public IEnumerable<string> GetLogfiles(CancellationToken cancellationToken)
    {
        var logPath = GetLogPath();

        var files = Directory.GetFiles(logPath);

        return files.Select(f => Path.GetFileName(f));
    }

    /// <inheritdoc/>
    public async ValueTask<string> ReadLogfileAsync(string logFileSelected, CancellationToken cancellationToken)
    {
        var logPath = GetLogPath();

        string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
        foreach (char c in invalid)
        {
            logFileSelected = logFileSelected.Replace(c.ToString(), "");
        }

        var path = Path.Combine(logPath, logFileSelected);
        if (path.StartsWith(logPath))
        {
            using var fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var bs = new BufferedStream(fs);
            using var sr = new StreamReader(bs);

            return await sr.ReadToEndAsync();
        }
        else
        {
            _logger.Warn($"Invalid Log file path {path}");
        }

        return string.Empty;
    }

    /// <inheritdoc/>
    public async ValueTask CleanLogfileAsync(string logFileSelected, CancellationToken cancellationToken)
    {
        var logPath = GetLogPath();

        string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
        foreach (char c in invalid)
        {
            logFileSelected = logFileSelected.Replace(c.ToString(), "");
        }

        var path = Path.Combine(logPath, logFileSelected);
        if (path.StartsWith(logPath))
        {
            await File.WriteAllTextAsync(path, string.Empty, cancellationToken);
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
    private static string GetLogPath()
    {
        var bin = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        var path = Path.Combine(bin, "logs");

        return path;
    }
}
