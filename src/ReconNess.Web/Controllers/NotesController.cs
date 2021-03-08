using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReconNess.Core.Services;
using ReconNess.Web.Dtos;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Web.Controllers
{
    [Authorize]
    [Produces("application/json")]
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

        /// <summary>
        /// Save a note for the rootdomain.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/notes/rootdomain/{targetName}/{rootDomainName}
        ///     {
        ///         "notes": "The new note"
        ///     }
        ///
        /// </remarks>
        /// <param name="targetName">The target name</param>
        /// <param name="rootDomainName">The rootdomain name</param>
        /// <param name="noteDto">The Note dto</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <response code="204">No Content</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">If the user is not authenticate</response>
        /// <response code="404">Not Found</response>
        [HttpPost("rootdomain/{targetName}/{rootDomainName}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]        
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Save a note for the subdomain.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/notes/subdomain/{target}/{rootDomainName}/{subdomain}
        ///     {
        ///         "notes": "The new note"
        ///     }
        ///
        /// </remarks>
        /// <param name="targetName">The target name</param>
        /// <param name="rootDomainName">The rootdomain name</param>
        /// <param name="subdomainName">The subdomain name</param>
        /// <param name="noteDto">The note dto</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <response code="204">No Content</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">If the user is not authenticate</response>
        /// <response code="404">Not Found</response>
        [HttpPost("subdomain/{targetName}/{rootDomainName}/{subdomainName}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]        
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
