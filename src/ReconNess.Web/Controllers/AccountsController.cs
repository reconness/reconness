using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ReconNess.Core.Services;
using ReconNess.Entities;
using ReconNess.Web.Dtos;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly INotificationService notificationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountsController" /> class
        public AccountsController(IMapper mapper, 
            INotificationService notificationService)
        {
            this.mapper = mapper;
            this.notificationService = notificationService;
        }

        // GET api/account/notification
        [HttpGet("notification")]
        public async Task<IActionResult> Notification(CancellationToken cancellationToken)
        {
            var notifications = await this.notificationService.GetByCriteriaAsync(n => !n.Deleted, cancellationToken);

            var notificationDto = this.mapper.Map<Notification, NotificationDto>(notifications);

            return Ok(notificationDto);
        }

        // POST api/account/saveNotification
        [HttpPost("saveNotification")]
        public async Task<IActionResult> SaveNotification([FromBody] NotificationDto notificationDto, CancellationToken cancellationToken)
        {
            var notification = this.mapper.Map<NotificationDto, Notification>(notificationDto);

            await this.notificationService.SaveNotificationAsync(notification, cancellationToken);

            return NoContent();
        }
    }
}
