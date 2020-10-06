using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Core.Providers
{
    /// <summary>
    /// 
    /// </summary>
    public interface IVersionProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<string> GetLatestVersionAsync(CancellationToken cancellationToken);
    }
}
