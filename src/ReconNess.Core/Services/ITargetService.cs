using ReconNess.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Core.Services
{
    /// <summary>
    /// The interface ILabelService
    /// </summary>
    public interface ITargetService : IService<Target>, ISaveTerminalOutputParseService
    {
        /// <summary>
        /// Obtain the target with the include references
        /// </summary>
        /// <param name="targetName">The target name</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Target> GetTargetWithIncludeAsync(string targetName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete target with all the root domains and subdomains
        /// </summary>
        /// <param name="target">The target</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task DeleteTargetAsync(Target target, CancellationToken cancellationToken = default);
    }
}
