using ReconNess.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Core.Services
{
    /// <summary>
    /// The interface INotesService
    /// </summary>
    public interface INotificationService : IService<Notification>
    {
        /// <summary>
        /// Save the account notification
        /// </summary>
        /// <param name="notification">The notification data</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A Task</returns>
        Task SaveNotificationAsync(Notification notification, CancellationToken cancellationToken);
    }
}
