using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Core.Providers
{
    /// <summary>
    /// The interface IVersionProvider
    /// </summary>
    public interface IVersionProvider
    {
        /// <summary>
        /// Obtain the current ReconNess version
        /// </summary>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A Task</returns>
        Task<string> GetCurrentVersionAsync(CancellationToken cancellationToken);
    }
}
