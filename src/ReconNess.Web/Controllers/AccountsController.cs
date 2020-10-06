using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using ReconNess.Core.Providers;
using ReconNess.Core.Services;
using ReconNess.Entities;
using ReconNess.Web.Dtos;
using System.IO;
using System.Linq;
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
        private readonly IVersionProvider currentVersionProvider;

        private readonly IWebHostEnvironment webHostEnvironment;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountsController" /> class
        public AccountsController(IMapper mapper,
            INotificationService notificationService,
            IVersionProvider currentVersionProvider,
            IWebHostEnvironment webHostEnvironment)
        {
            this.mapper = mapper;
            this.notificationService = notificationService;
            this.currentVersionProvider = currentVersionProvider;
            this.webHostEnvironment = webHostEnvironment;
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
            var bin = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            var path = Path.Combine(bin, "logs").Substring(6);

            var files = Directory.GetFiles(path);
            
            return Ok(files.Select(f => Path.GetFileName(f)));
        }

        // GET api/accounts/readLogfile{logFileSelected}
        [HttpGet("readLogfile/{logFileSelected}")]
        public async Task<IActionResult> ReadLogfile(string logFileSelected, CancellationToken cancellationToken)
        {
            var bin = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            var path = Path.Combine(bin, "logs", logFileSelected).Substring(6);

            using (FileStream fs = System.IO.File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (BufferedStream bs = new BufferedStream(fs))
            using (StreamReader sr = new StreamReader(bs))
            {
                return Ok(await sr.ReadToEndAsync());               
            }
        }

        // POST api/accounts/cleanLogfile
        [HttpPost("cleanLogfile")]
        public IActionResult CleanLogfile([FromBody] AccountLogFileDto accountLogFileDto, CancellationToken cancellationToken)
        {
            var bin = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            var path = Path.Combine(bin, "logs", accountLogFileDto.LogFileSelected).Substring(6);

            System.IO.File.WriteAllText(path, string.Empty);

            return NoContent();
        }
    }
}
