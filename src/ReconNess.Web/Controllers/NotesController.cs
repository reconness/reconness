using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReconNess.Core.Services;
using ReconNess.Entities;
using ReconNess.Web.Dtos;
using System;
using System.Collections.Generic;
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
        private readonly IMapper mapper;

        private readonly INotesService notesService;
        private readonly ITargetService targetService;
        private readonly IRootDomainService rootDomainService;
        private readonly ISubdomainService subdomainService;
        private readonly IEventTrackService eventTrackService;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotesController" /> class
        /// </summary>
        /// <param name="mapper"><see cref="IMapper"/></param>
        /// <param name="notesService"><see cref="INotesService"/></param>
        /// <param name="targetService"><see cref="ITargetService"/></param>
        /// <param name="rootDomainService"><see cref="IRootDomainService"/></param>
        /// <param name="subdomainService"><see cref="ISubdomainService"/></param>
        /// <param name="eventTrackService"><see cref="IEventTrackService"/></param>
        public NotesController(
            IMapper mapper,
            INotesService notesService,
            ITargetService targetService,
            IRootDomainService rootDomainService,
            ISubdomainService subdomainService,
            IEventTrackService eventTrackService)
        {
            this.mapper = mapper;
            this.notesService = notesService;
            this.targetService = targetService;
            this.rootDomainService = rootDomainService;
            this.subdomainService = subdomainService;
            this.eventTrackService = eventTrackService;
        }

        /// <summary>
        /// Obtain the list of notes from a target.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET api/notes/target/{targetName}
        ///
        /// </remarks>
        /// <param name="targetName">The target</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The list of agents</returns>
        /// <response code="200">Returns the list of agents</response>
        /// <response code="401">If the user is not authenticate</response>
        [HttpGet("target/{targetName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetTargetNotes(string targetName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(targetName))
            {
                return BadRequest();
            }

            var target = await this.targetService.GetAllQueryableByCriteria(t => t.Name == targetName)
                    .Include(t => t.Notes)
                .FirstOrDefaultAsync(cancellationToken);

            if (target == null)
            {
                return NotFound();
            }

            var notesDto = this.mapper.Map<ICollection<Note>, List<NoteDto>>(target.Notes);

            return Ok(notesDto);
        }

        /// <summary>
        /// Save a note for the target.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/notes/target/{targetName}
        ///     {
        ///         "comment": "The new note"
        ///     }
        ///
        /// </remarks>
        /// <param name="targetName">The target name</param>
        /// <param name="noteDto">The Note dto</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <response code="204">No Content</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">If the user is not authenticate</response>
        /// <response code="404">Not Found</response>
        [HttpPost("target/{targetName}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddTargetNote(string targetName, [FromBody] NoteAddDto noteDto, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(targetName))
            {
                return BadRequest();
            }

            var target = await this.targetService.GetAllQueryableByCriteria(t => t.Name == targetName)
                    .Include(t => t.Notes)
                .FirstOrDefaultAsync(cancellationToken);

            if (target == null)
            {
                return NotFound();
            }

            var noteEntity = await this.notesService.AddTargetCommentAsync(target, noteDto.Comment, cancellationToken);

            await this.eventTrackService.AddAsync(new EventTrack
            {
                Target = target,
                Data = $"Note '{noteDto.Comment}' added"
            }, cancellationToken);

            var noteDtoResponse = this.mapper.Map<Note, NoteDto>(noteEntity);


            return Ok(noteDtoResponse);
        }

        /// <summary>
        /// Delete a target note.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE api/notes/{id}/target/{targetName}
        ///
        /// </remarks>
        /// <param name="id">The notes id</param>
        /// <param name="targetName">The target</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <response code="204">No Content</response>
        /// <response code="401">If the user is not authenticate</response>
        /// <response code="404">Not Found</response>
        [HttpDelete("{id}/target/{targetName}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTargetNote([FromRoute] Guid id, string targetName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(targetName))
            {
                return BadRequest();
            }

            var target = await this.targetService.GetByCriteriaAsync(t => t.Name == targetName, cancellationToken);
            if (target == null)
            {
                return NotFound();
            }

            var note = await this.notesService.GetByCriteriaAsync(n => n.Id == id && n.Target == target, cancellationToken);
            if (note == null)
            {
                return NotFound();
            }

            await this.notesService.DeleteAsync(note, cancellationToken);

            await this.eventTrackService.AddAsync(new EventTrack
            {
                Target = target,                
                Data = $"Note '{note.Comment}' deleted"
            }, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// Obtain the list of notes from a rootdomain.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET api/notes/rootdomain/{targetName}/{rootDomainName}
        ///
        /// </remarks>
        /// <param name="targetName">The target</param>
        /// <param name="rootDomainName">The rootdomain</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The list of agents</returns>
        /// <response code="200">Returns the list of agents</response>
        /// <response code="401">If the user is not authenticate</response>
        [HttpGet("rootdomain/{targetName}/{rootDomainName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetRootDomainNotes(string targetName, string rootDomainName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(targetName) || string.IsNullOrEmpty(rootDomainName))
            {
                return BadRequest();
            }

            var target = await this.targetService.GetByCriteriaAsync(t => t.Name == targetName, cancellationToken);
            if (target == null)
            {
                return NotFound();
            }

            var rootDomain = await this.rootDomainService
                .GetAllQueryableByCriteria(r => r.Target == target && r.Name == rootDomainName)
                    .Include(t => t.Notes)
                .FirstOrDefaultAsync(cancellationToken);

            if (rootDomain == null)
            {
                return NotFound();
            }

            var notesDto = this.mapper.Map<ICollection<Note>, List<NoteDto>>(rootDomain.Notes);

            return Ok(notesDto);
        }

        /// <summary>
        /// Save a note for the rootdomain.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/notes/rootdomain/{targetName}/{rootDomainName}
        ///     {
        ///         "comment": "The new note"
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
        public async Task<IActionResult> AddRootDomainNote(string targetName, string rootDomainName, [FromBody] NoteAddDto noteDto, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(targetName) || string.IsNullOrEmpty(rootDomainName))
            {
                return BadRequest();
            }

            var target = await this.targetService.GetByCriteriaAsync(t => t.Name == targetName, cancellationToken);
            if (target == null)
            {
                return NotFound();
            }

            var rootDomain = await this.rootDomainService
                .GetAllQueryableByCriteria(r => r.Target == target && r.Name == rootDomainName)
                    .Include(t => t.Notes)
                .FirstOrDefaultAsync(cancellationToken);

            if (rootDomain == null)
            {
                return NotFound();
            }

            await this.notesService.AddRootdomainCommentAsync(rootDomain, noteDto.Comment, cancellationToken);

            await this.eventTrackService.AddAsync(new EventTrack
            {
                Target = target,
                RootDomain = rootDomain,
                Data = $"Note '{noteDto.Comment}' added"
            }, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// Delete a rootdomain note.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE api/notes/{id}/rootdomain/{targetName}/{rootDomainName}
        ///
        /// </remarks>
        /// <param name="id">The notes id</param>
        /// <param name="targetName">The target</param>
        /// <param name="rootDomainName">The rootdomain</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <response code="204">No Content</response>
        /// <response code="401">If the user is not authenticate</response>
        /// <response code="404">Not Found</response>
        [HttpDelete("{id}/rootdomain/{targetName}/{rootDomainName}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteRootDomainNote([FromRoute] Guid id, string targetName, string rootDomainName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(targetName) || string.IsNullOrEmpty(rootDomainName))
            {
                return BadRequest();
            }

            var target = await this.targetService.GetByCriteriaAsync(t => t.Name == targetName, cancellationToken);
            if (target == null)
            {
                return NotFound();
            }

            var rootDomain = await this.rootDomainService.GetByCriteriaAsync(r => r.Target == target && r.Name == rootDomainName, cancellationToken);
            if (rootDomain == null)
            {
                return NotFound();
            }

            var note = await this.notesService.GetByCriteriaAsync(n => n.Id == id && n.RootDomain == rootDomain, cancellationToken);
            if (note == null)
            {
                return NotFound();
            }

            await this.notesService.DeleteAsync(note, cancellationToken);

            await this.eventTrackService.AddAsync(new EventTrack
            {
                Target = target,
                RootDomain = rootDomain,
                Data = $"Note '{note.Comment}' deleted"
            }, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// Obtain the list of notes from a subdomain.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET api/notes/subdomain/{targetName}/{rootDomainName}/{subdomainName}
        ///
        /// </remarks>
        /// <param name="targetName">The target</param>
        /// <param name="rootDomainName">The rootdomain</param>
        /// <param name="subdomainName">The subdomain</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The list of agents</returns>
        /// <response code="200">Returns the list of agents</response>
        /// <response code="401">If the user is not authenticate</response>
        [HttpGet("subdomain/{targetName}/{rootDomainName}/{subdomainName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetSubdomainNotes(string targetName, string rootDomainName, string subdomainName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(targetName) || string.IsNullOrEmpty(rootDomainName) || string.IsNullOrEmpty(subdomainName))
            {
                return BadRequest();
            }

            var target = await this.targetService.GetByCriteriaAsync(t => t.Name == targetName, cancellationToken);
            if (target == null)
            {
                return NotFound();
            }

            var rootDomain = await this.rootDomainService.GetByCriteriaAsync(r => r.Target == target && r.Name == rootDomainName, cancellationToken);
            if (rootDomain == null)
            {
                return NotFound();
            }

            var subdomain = await this.subdomainService
                .GetAllQueryableByCriteria(s => s.RootDomain == rootDomain && s.Name == subdomainName)
                    .Include(s => s.Notes)
                .FirstOrDefaultAsync(cancellationToken);

            if (subdomain == null)
            {
                return NotFound();
            }

            var notesDto = this.mapper.Map<ICollection<Note>, List<NoteDto>>(subdomain.Notes);

            return Ok(notesDto);
        }

        /// <summary>
        /// Save a note for the subdomain.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/notes/subdomain/{target}/{rootDomainName}/{subdomain}
        ///     {
        ///         "comment": "The new note"
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
        public async Task<IActionResult> AddSubdomainNote(string targetName, string rootDomainName, string subdomainName, [FromBody] NoteAddDto noteDto, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(targetName) || string.IsNullOrEmpty(rootDomainName) || string.IsNullOrEmpty(subdomainName))
            {
                return BadRequest();
            }

            var target = await this.targetService.GetByCriteriaAsync(t => t.Name == targetName, cancellationToken);
            if (target == null)
            {
                return NotFound();
            }

            var rootDomain = await this.rootDomainService.GetByCriteriaAsync(r => r.Target == target && r.Name == rootDomainName, cancellationToken);
            if (rootDomain == null)
            {
                return NotFound();
            }

            var subdomain = await this.subdomainService
                .GetAllQueryableByCriteria(s => s.RootDomain == rootDomain && s.Name == subdomainName)
                    .Include(s => s.Notes)
                .FirstOrDefaultAsync(cancellationToken);

            if (subdomain == null)
            {
                return NotFound();
            }

            await this.notesService.AddSubdomainCommentAsync(subdomain, noteDto.Comment, cancellationToken);

            await this.eventTrackService.AddAsync(new EventTrack
            {
                Target = target,
                RootDomain = rootDomain,
                Subdomain = subdomain,
                Data = $"Note '{noteDto.Comment}' added"
            }, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// Delete a subdomain note.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE api/notes/{id}/subdomain/{targetName}/{rootDomainName}/{subdomainName}
        ///
        /// </remarks>
        /// <param name="id">The notes id</param>
        /// <param name="targetName">The target</param>
        /// <param name="rootDomainName">The rootdomain</param>
        /// <param name="subdomainName">The subdomain</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <response code="204">No Content</response>
        /// <response code="401">If the user is not authenticate</response>
        /// <response code="404">Not Found</response>
        [HttpDelete("{id}/subdomain/{targetName}/{rootDomainName}/{subdomainName}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletSubdomainNote([FromRoute] Guid id, string targetName, string rootDomainName, string subdomainName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(targetName) || string.IsNullOrEmpty(rootDomainName) || string.IsNullOrEmpty(subdomainName))
            {
                return BadRequest();
            }

            var target = await this.targetService.GetByCriteriaAsync(t => t.Name == targetName, cancellationToken);
            if (target == null)
            {
                return NotFound();
            }

            var rootDomain = await this.rootDomainService.GetByCriteriaAsync(r => r.Target == target && r.Name == rootDomainName, cancellationToken);
            if (rootDomain == null)
            {
                return NotFound();
            }

            var subdomain = await this.subdomainService.GetByCriteriaAsync(s => s.RootDomain == rootDomain && s.Name == subdomainName, cancellationToken);
            if (subdomain == null)
            {
                return NotFound();
            }

            var note = await this.notesService.GetByCriteriaAsync(n => n.Id == id && n.Subdomain == subdomain, cancellationToken);
            if (note == null)
            {
                return NotFound();
            }

            await this.notesService.DeleteAsync(note, cancellationToken);

            await this.eventTrackService.AddAsync(new EventTrack
            {
                Target = target,
                RootDomain = rootDomain,
                Subdomain = subdomain,
                Data = $"Note '{note.Comment}' deleted"
            }, cancellationToken);

            return NoContent();
        }
    }
}
