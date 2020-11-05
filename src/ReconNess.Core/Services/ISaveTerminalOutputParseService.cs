using ReconNess.Core.Models;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// 
/// </summary>
namespace ReconNess.Core.Services
{
    public interface ISaveTerminalOutputParseService<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="terminalOutputParse"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task SaveTerminalOutputParseAsync(T type, bool activeNotification, ScriptOutput terminalOutputParse, CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="agentName">Agent Name</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task UpdateAgentRanAsync(T type, string agentName, CancellationToken cancellationToken = default);
    }
}
