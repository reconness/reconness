using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReconNess.Core.Providers;
using ReconNess.Core.Services;
using ReconNess.Entities;
using ReconNess.Web.Dtos;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Web.Controllers
{
    [Authorize]
    [Produces("application/json")]
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
        /// </summary>
        /// <param name="mapper"><see cref="IMapper"/></param>
        /// <param name="notificationService"><see cref="INotificationService"/></param>
        /// <param name="currentVersionProvider"><see cref="IVersionProvider"/></param>
        /// <param name="logsProvider"><see cref="ILogsProvider"/></param>
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

        /// <summary>
        /// Obtain the notifications configuration.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET api/accounts/notification
        ///
        /// </remarks>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The notifications configuration</returns>
        /// <response code="200">Returns the notifications configuration</response>
        /// <response code="401">If the user is not authenticate</response>
        [HttpGet("notification")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Notification(CancellationToken cancellationToken)
        {
            var notification = await this.notificationService.GetAllQueryableByCriteria(n => !n.Deleted)
                    .AsNoTracking()
                .SingleOrDefaultAsync(cancellationToken);

            var notificationDto = this.mapper.Map<Notification, NotificationDto>(notification);

            return Ok(notificationDto);
        }

        /// <summary>
        /// Save the notifications configuration.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/accounts/saveNotification
        ///     {
        ///         "url": "https://hooks.slack.com/services/G04245NFL1B/B015GK97Y81/EoivoBWW1RSpYcSOdFyB8VZa",
        ///         "method": "POST",
        ///         "payload": "{\"text\":\"{{notification}}\"}",
        ///         "rootDomainPayload": "Rootdomain Payload",
        ///         "subdomainPayload": "Subdomain Payload",
        ///         "ipAddressPayload": "IpAddress Payload",
        ///         "isAlivePayload": "IsAlive Payload",
        ///         "hasHttpOpenPayload": "Has HttpOpen Payload",
        ///         "takeoverPayload": "Takeover Payload",
        ///         "directoryPayload": "Directory Payload",
        ///         "servicePayload": "Service Payload",
        ///         "notePayload": "Note Payload",
        ///         "technologyPayload": "Technology Payload",
        ///         "screenshotPayload": "Screenshot Payload"
        ///     }
        ///
        /// </remarks>
        /// <param name="notificationDto">the notifications configuration to be saved</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <response code="204">No Content</response>
        /// <response code="401">If the user is not authenticate</response>        
        [HttpPost("saveNotification")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> SaveNotification([FromBody] NotificationDto notificationDto, CancellationToken cancellationToken)
        {
            var notification = this.mapper.Map<NotificationDto, Notification>(notificationDto);

            await this.notificationService.SaveNotificationAsync(notification, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// Obtain the last ReconNess Version released.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET api/accounts/latestVersion	
        ///
        /// </remarks>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The last ReconNess Version released</returns>
        /// <response code="200">Returns the last ReconNess Version released</response>
        /// <response code="401">If the user is not authenticate</response>
        [HttpGet("latestVersion")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> LatestVersion(CancellationToken cancellationToken)
        {
            var latestVersion = await this.currentVersionProvider.GetLatestVersionAsync(cancellationToken);

            return Ok(latestVersion);
        }

        /// <summary>
        /// Obtain the current ReconNess Version.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET api/accounts/currentVersion	
        ///
        /// </remarks>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The current ReconNess Version</returns>
        /// <response code="200">Returns the current ReconNess Version</response>
        /// <response code="401">If the user is not authenticate</response>
        [HttpGet("currentVersion")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CurrentVersion(CancellationToken cancellationToken)
        {
            var currentVersion = await this.currentVersionProvider.GetCurrentVersionAsync(cancellationToken);

            return Ok(currentVersion);
        }

        /// <summary>
        /// Obtain the list of log files.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET api/accounts/logfiles
        ///
        /// </remarks>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The list of log files</returns>
        /// <response code="200">Returns the list of log files</response>
        /// <response code="401">If the user is not authenticate</response>        
        [HttpGet("logfiles")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Logfiles(CancellationToken cancellationToken)
        {
            var logFiles = this.logsProvider.GetLogfiles(cancellationToken);

            return Ok(logFiles);
        }

        /// <summary>
        /// Read the log file data.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET api/accounts/readLogfile/{logFileSelected}
        ///
        /// </remarks>
        /// <param name="logFileSelected">The log file to read</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>Read the log file data</returns>
        /// <response code="200">Returns the log file data</response>
        /// <response code="401">If the user is not authenticate</response>
        [HttpGet("readLogfile/{logFileSelected}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ReadLogfile(string logFileSelected, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(logFileSelected))
            {
                return BadRequest();
            }

            var readLogFile = await this.logsProvider.ReadLogfileAsync(logFileSelected, cancellationToken);

            return Ok(readLogFile);
        }

        /// <summary>
        /// Clean the log file data.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/accounts/cleanLogfile
        ///     {
        ///         "logFileSelected": "logfilename"
        ///     }
        ///
        /// </remarks>
        /// <param name="accountLogFileDto">The log file selected to be cleaned</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <response code="204">No Content</response>
        /// <response code="401">If the user is not authenticate</response>
        [HttpPost("cleanLogfile")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
