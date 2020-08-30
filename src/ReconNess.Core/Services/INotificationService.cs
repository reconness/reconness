using ReconNess.Core.Models;
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

        /// <summary>
        /// Send a notification
        /// </summary>
        /// <param name="agentPayload">The agent payload to send in the notification payload</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns></returns>
        Task SendAsync(string agentPayload, CancellationToken cancellationToken);


        /// <summary>
        /// Send notifications if it is actived
        /// </summary>
        /// <param name="notificationType">Notification Type</param>
        /// <param name="replaces">The replacement values</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns></returns>
        Task SendAsync(NotificationType notificationType, (string, string)[] replaces, CancellationToken cancellationToken = default);
    }
}
