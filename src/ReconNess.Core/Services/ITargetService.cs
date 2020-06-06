using ReconNess.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Core.Services
{
    /// <summary>
    /// The interface ILabelService
    /// </summary>
    public interface ITargetService : IService<Target>
    {
        Task DeleteTargetAsync(Target target, CancellationToken cancellationToken);
    }
}
