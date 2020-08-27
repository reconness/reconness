using ReconNess.Core.Models;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// 
/// </summary>
namespace ReconNess.Core.Services
{
    public interface ISaveTerminalOutputParseService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="agentRunner"></param>
        /// <param name="terminalOutputParse"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task SaveTerminalOutputParseAsync(AgentRunner agentRunner, ScriptOutput terminalOutputParse, CancellationToken cancellationToken = default);
    }
}
