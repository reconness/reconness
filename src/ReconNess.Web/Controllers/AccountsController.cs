using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReconNess.Core.Providers;
using ReconNess.Core.Services;
using ReconNess.Entities;
using ReconNess.Web.Dtos;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly INotificationService notificationService;
        private readonly IVersionProvider currentVersionProvider;
        private readonly ILogsProvider logsProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountsController" /> class
        public AccountsController(IMapper mapper,
            INotificationService notificationService,
            IVersionProvider currentVersionProvider,
            ILogsProvider logsProvider)
        {
            this.mapper = mapper;
            this.notificationService = notificationService;
            this.currentVersionProvider = currentVersionProvider;
            this.logsProvider = logsProvider;
        }

        // GET api/accounts/notification
        [HttpGet("notification")]
        public async Task<IActionResult> Notification(CancellationToken cancellationToken)
        {
            var notification = await this.notificationService.GetByCriteriaAsync(n => !n.Deleted, cancellationToken);

            var notificationDto = this.mapper.Map<Notification, NotificationDto>(notification);

            return Ok(notificationDto);
        }

        // POST api/accounts/saveNotification
        [HttpPost("saveNotification")]
        public async Task<IActionResult> SaveNotification([FromBody] NotificationDto notificationDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var notification = this.mapper.Map<NotificationDto, Notification>(notificationDto);

            await this.notificationService.SaveNotificationAsync(notification, cancellationToken);

            return NoContent();
        }

        // GET api/accounts/latestVersion	
        [HttpGet("latestVersion")]
        public async Task<IActionResult> LatestVersion(CancellationToken cancellationToken)
        {
            var latestVersion = await this.currentVersionProvider.GetLatestVersionAsync(cancellationToken);

            return Ok(latestVersion);
        }

        // GET api/accounts/currentVersion
        [HttpGet("currentVersion")]
        public async Task<IActionResult> CurrentVersion(CancellationToken cancellationToken)
        {
            var currentVersion = await this.currentVersionProvider.GetCurrentVersionAsync(cancellationToken);

            return Ok(currentVersion);
        }

        // GET api/accounts/logfiles
        [HttpGet("logfiles")]
        public IActionResult Logfiles(CancellationToken cancellationToken)
        {
            var logFiles = this.logsProvider.GetLogfiles(cancellationToken);

            return Ok(logFiles);
        }

        // GET api/accounts/readLogfile{logFileSelected}
        [HttpGet("readLogfile/{logFileSelected}")]
        public async Task<IActionResult> ReadLogfile(string logFileSelected, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(logFileSelected))
            {
                return BadRequest();
            }

            var readLogFile = await this.logsProvider.ReadLogfileAsync(logFileSelected, cancellationToken);

            return Ok(readLogFile);
        }

        // POST api/accounts/cleanLogfile
        [HttpPost("cleanLogfile")]
        public async Task<IActionResult> CleanLogfile([FromBody] AccountLogFileDto accountLogFileDto, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(accountLogFileDto.LogFileSelected))
            {
                return BadRequest();
            }

            await this.logsProvider.CleanLogfileAsync(accountLogFileDto.LogFileSelected, cancellationToken);

            return NoContent();
        }
    }
}
