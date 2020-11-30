using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReconNess.Core.Services;
using ReconNess.Web.Dtos;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly INotesService notesService;
        private readonly ITargetService targetService;
        private readonly IRootDomainService rootDomainService;
        private readonly ISubdomainService subdomainService;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotesController" /> class
        /// </summary>
        /// <param name="notesService"><see cref="INotesService"/></param>
        /// <param name="targetService"><see cref="ITargetService"/></param>
        /// <param name="rootDomainService"><see cref="IRootDomainService"/></param>
        /// <param name="subdomainService"><see cref="ISubdomainService"/></param>
        public NotesController(
            INotesService notesService,
            ITargetService targetService,
            IRootDomainService rootDomainService,
            ISubdomainService subdomainService)
        {
            this.notesService = notesService;
            this.targetService = targetService;
            this.rootDomainService = rootDomainService;
            this.subdomainService = subdomainService;
        }

        // GET api/notes/rootdomain/{targetName}/{rootDomainName}
        [HttpPost("rootdomain/{targetName}/{rootDomainName}")]
        public async Task<IActionResult> SaveRootDomainNotes(string targetName, string rootDomainName, [FromBody] NoteDto noteDto, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(targetName) || string.IsNullOrEmpty(rootDomainName))
            {
                return BadRequest();
            }

            var target = await this.targetService.GetTargetNotTrackingAsync(t => t.Name == targetName, cancellationToken);
            if (target == null)
            {
                return NotFound();
            }

            var rootDomain = await this.rootDomainService
                .GetAllQueryableByCriteria(r => r.Target == target && r.Name == rootDomainName, cancellationToken)
                    .Include(t => t.Notes)
                .SingleAsync(cancellationToken);

            if (rootDomain == null)
            {
                return NotFound();
            }

            await this.notesService.SaveRootdomainNotesAsync(rootDomain, noteDto.Notes, cancellationToken);

            return NoContent();
        }

        // GET api/notes/subdomain/{target}/{rootDomainName}/{subdomain}
        [HttpPost("subdomain/{targetName}/{rootDomainName}/{subdomainName}")]
        public async Task<IActionResult> SaveSubdomainNotes(string targetName, string rootDomainName, string subdomainName, [FromBody] NoteDto noteDto, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(targetName) || string.IsNullOrEmpty(rootDomainName) || string.IsNullOrEmpty(subdomainName))
            {
                return BadRequest();
            }

            var target = await this.targetService.GetTargetNotTrackingAsync(t => t.Name == targetName, cancellationToken);
            if (target == null)
            {
                return NotFound();
            }

            var rootDomain = await this.rootDomainService.GetRootDomainNoTrackingAsync(r => r.Target == target && r.Name == rootDomainName, cancellationToken);
            if (rootDomain == null)
            {
                return NotFound();
            }

            var subdomain = await this.subdomainService
                .GetAllQueryableByCriteria(s => s.RootDomain == rootDomain && s.Name == subdomainName, cancellationToken)
                    .Include(s => s.Notes)
                .SingleAsync(cancellationToken);

            if (subdomain == null)
            {
                return NotFound();
            }

            await this.notesService.SaveSubdomainNotesAsync(subdomain, noteDto.Notes, cancellationToken);

            return NoContent();
        }
    }
}
