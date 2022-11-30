using ReconNess.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Application.Providers;

/// <summary>
/// This interface provide access to the notification 3-party
/// </summary>
public interface INotificationProvider
{
    /// <summary>
    /// Send a notification
    /// </summary>
    /// <param name="notification">The <see cref="Notification"/> entity</param>
    /// <param name="payload">The payload to send</param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>A Task</returns>
    Task SendNotificationAsync(Notification notification, string payload, CancellationToken cancellationToken = default);
}
