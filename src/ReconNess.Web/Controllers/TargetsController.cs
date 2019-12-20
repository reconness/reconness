using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReconNess.Core.Services;
using ReconNess.Entities;
using ReconNess.Web.Dtos;

namespace ReconNess.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TargetsController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ITargetService targetService;

        public TargetsController(IMapper mapper, ITargetService targetService)
        {
            this.mapper = mapper;
            this.targetService = targetService;
        }

        // GET api/targets
        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var targets = await this.targetService.GetAllByCriteriaAsync(t => !t.Deleted, cancellationToken);

            return Ok(this.mapper.Map<List<Target>, List<TargetDto>>(targets));
        }

        // GET api/targets/{targetName}
        [HttpGet("{targetName}")]
        public async Task<IActionResult> Get(string targetName, CancellationToken cancellationToken)
        {
            var target = await this.targetService.GetTargetWithSubdomainsAsync(t => t.Name == targetName, cancellationToken);
            if (target == null)
            {
                return NotFound();
            }

            return Ok(this.mapper.Map<Target, TargetDto>(target));
        }

        // POST api/targets
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TargetDto targetDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var target = this.mapper.Map<TargetDto, Target>(targetDto);

            var newtarget = await this.targetService.AddAsync(target, cancellationToken);

            return CreatedAtAction(nameof(Get), new { id = newtarget.Id }, newtarget);
        }

        // PUT api/targets/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] TargetDto targetDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var target = await this.targetService.FindAsync(id, cancellationToken);
            if (target == null)
            {
                return NotFound();
            }

            var editedtarget = this.mapper.Map<TargetDto, Target>(targetDto);

            target.Name = editedtarget.Name;
            target.RootDomain = editedtarget.RootDomain;
            target.BugBountyProgramUrl = editedtarget.BugBountyProgramUrl;
            target.IsPrivate = editedtarget.IsPrivate;
            target.InScope = editedtarget.InScope;
            target.OutOfScope = editedtarget.OutOfScope;

            await this.targetService.UpdateAsync(target, cancellationToken);

            return NoContent();
        }

        // DELETE api/targets/{targetName}
        [HttpDelete("{targetName}")]
        public async Task<IActionResult> Delete(string targetName, CancellationToken cancellationToken)
        {
            var target = await this.targetService.GetTargetWithSubdomainsAsync(t => t.Name == targetName, cancellationToken);
            if (target == null)
            {
                return NotFound();
            }

            await this.targetService.DeleteTargetAsync(target, cancellationToken);

            return NoContent();
        }

        // DELETE api/targets/subdomain/{targetName}
        [HttpDelete("subdomain/{targetName}")]
        public async Task<IActionResult> DeleteAllSubdomains(string targetName, CancellationToken cancellationToken)
        {
            var target = await this.targetService.GetTargetWithSubdomainsAsync(t => t.Name == targetName, cancellationToken);
            if (target == null)
            {
                return NotFound();
            }

            await this.targetService.DeleteSubdomainsAsync(target, cancellationToken);

            return NoContent();
        }

        // POST api/targets/subdomain/{targetName}
        [HttpPost("subdomain/{targetName}")]
        public async Task<IActionResult> UploadSubdomains(string targetName, IFormFile file, CancellationToken cancellationToken)
        {
            if (file.Length == 0)
            {
                return BadRequest();
            }

            var target = await this.targetService.GetTargetWithSubdomainsAsync(t => t.Name == targetName, cancellationToken);
            if (target == null)
            {
                return NotFound();
            }

            try
            {
                var path = Path.GetTempFileName();
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var subdomains = System.IO.File.ReadAllLines(path).ToList();
                await this.targetService.UploadSubdomainsAsync(target, subdomains);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return NoContent();
        }
    }
}
