using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReconNess.Core.Services;
using ReconNess.Entities;
using ReconNess.Web.Dtos;

namespace ReconNess.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly INotesService notesService;
        private readonly ITargetService targetService;
        private readonly ISubdomainService subdomainService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="notesService"></param>
        /// <param name="targetService"></param>
        /// <param name="subdomainService"></param>
        public NotesController(
            IMapper mapper,
            INotesService notesService,
            ITargetService targetService,
            ISubdomainService subdomainService)
        {
            this.mapper = mapper;
            this.notesService = notesService;
            this.targetService = targetService;
            this.subdomainService = subdomainService;
        }

        // GET api/notes/target/{targetName}
        [HttpPost("target/{targetName}")]
        public async Task<IActionResult> SaveTargetNotes(string targetName, [FromBody] NoteDto noteDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var target = await this.targetService.GetAllQueryableByCriteria(t => t.Name == targetName, cancellationToken)
                .Include(t => t.Notes)
                .FirstOrDefaultAsync(cancellationToken);

            if (target == null)
            {
                return NotFound();
            }

            var notes = await this.notesService.SaveTargetNotesAsync(target, noteDto.Notes, cancellationToken);

            return Ok(this.mapper.Map<Note, NoteDto>(notes));
        }

        // GET api/notes/subdomain/{target}/{subdomain}
        [HttpPost("subdomain/{targetName}/{subdomainName}")]
        public async Task<IActionResult> SaveSubdomainNotes(string targetName, string subdomainName, [FromBody] NoteDto noteDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var target = await this.targetService.GetByCriteriaAsync(t => t.Name == targetName, cancellationToken);
            if (target == null)
            {
                return NotFound();
            }

            var subdomain = await this.subdomainService.GetByCriteriaAsync(s => s.Target == target && s.Name == subdomainName, cancellationToken);
            if (subdomain == null)
            {
                return NotFound();
            }

            var notes = await this.notesService.SaveSubdomainNotesAsync(subdomain, noteDto.Notes, cancellationToken);

            return Ok(this.mapper.Map<Note, NoteDto>(notes));
        }
    }
}
