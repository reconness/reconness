using System.Threading;
using System.Threading.Tasks;
using ReconNess.Core.Models;
using ReconNess.Entities;

namespace ReconNess.Core.Services
{
    /// <summary>
    /// The interface IScriptEngineService
    /// </summary>
    public interface IScriptEngineService
    {
        /// <summary>
        /// Inintialize the Agent running
        /// </summary>
        /// <param name="agent"></param>
        void InintializeAgent(Agent agent);

        /// <summary>
        /// Parse the terminal input and return what we need to save on database
        /// </summary>
        /// <param name="lineInput">The terminal output line</param>
        /// <param name="lineInputCount">the count of the terminal output line</param>
        /// <param name="script">Script to run by default is null</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>What we need to save on database</returns>
        Task<ScriptOutput> ParseInputAsync(string lineInput, int lineInputCount, string script = null, CancellationToken cancellationToken = default);
    }
}
