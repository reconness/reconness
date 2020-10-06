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
        /// Obtain the latest ReconNess version released
        /// </summary>
        /// <param name="cancellationToken">otification that operations should be canceled</param>
        /// <returns></returns>
        Task<string> GetLatestVersionAsync(CancellationToken cancellationToken);
    }
}
