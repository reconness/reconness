using ReconNess.Core;
using ReconNess.Core.Services;
using ReconNess.Entities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Services
{
    public class NotificationService : Service<Notification>, IService<Notification>, INotificationService
    {
        public NotificationService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

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
    }
}
