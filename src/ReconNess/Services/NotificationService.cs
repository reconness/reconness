using ReconNess.Core;
using ReconNess.Core.Services;
using ReconNess.Entities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Services
{
    /// <summary>
    /// This class implement <see cref="INotificationService"/>
    /// </summary>
    public class NotificationService : Service<Notification>, IService<Notification>, INotificationService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INotificationService" /> class
        /// </summary>
        /// <param name="unitOfWork"><see cref="IUnitOfWork"/></param>
        public NotificationService(IUnitOfWork unitOfWork) 
            : base(unitOfWork)
        {
        }

        /// <summary>
        /// <see cref="INotificationService.SaveNotificationAsync(Notification, CancellationToken)
        /// </summary>
        public async Task SaveNotificationAsync(Notification notification, CancellationToken cancellationToken)
        {
            var notif = (await this.GetAllAsync(cancellationToken)).FirstOrDefault();

            if (notif != null)
            {
                notif.Url = notification.Url;
                notif.Method = notification.Method;
                notif.Payload = notification.Payload;

                await this.UpdateAsync(notif, cancellationToken);               
            }
            else
            {
                await this.AddAsync(notification, cancellationToken);
            }
        }

        /// <summary>
        /// <see cref="INotificationService.SendAsync(string, CancellationToken)"/>
        /// </summary>
        public async Task SendAsync(string agentPayload, CancellationToken cancellationToken)
        {
            var notification = await this.GetByCriteriaAsync(n => n.Deleted!);
            if (notification == null)
            {
                return;
            }
        }
    }
}
