using ReconNess.Application.Models;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Application.Providers;

/// <summary>
/// The interface IScriptEngineService
/// </summary>
public interface IScriptEngineProvider
{
    /// <summary>
    /// Parse the terminal input and return what we need to save on database
    /// </summary>
    /// <param name="script">Script to run by default is null</param>
    /// <param name="lineInput">The terminal output line</param>
    /// <param name="lineInputCount">the count of the terminal output line</param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>What we need to save on database</returns>
    Task<ScriptOutput> TerminalOutputParseAsync(string script, string lineInput, int lineInputCount, CancellationToken cancellationToken = default);
}
