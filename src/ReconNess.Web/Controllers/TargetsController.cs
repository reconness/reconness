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
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TargetsController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ITargetService targetService;
        private readonly IRootDomainService rootDomainService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetsController" /> class
        /// </summary>
        /// <param name="mapper"><see cref="IMapper"/></param>
        /// <param name="targetService"><see cref="ITargetService"/></param>
        public TargetsController(
            IMapper mapper,
            ITargetService targetService,
            IRootDomainService rootDomainService)
        {
            this.mapper = mapper;
            this.targetService = targetService;
            this.rootDomainService = rootDomainService;
        }

        // GET api/targets
        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var targets = await this.targetService.GetAllQueryableByCriteria(t => !t.Deleted, cancellationToken)
                .Include(t => t.RootDomains)
                .ToListAsync(cancellationToken);

            return Ok(this.mapper.Map<List<Target>, List<TargetDto>>(targets));
        }

        // GET api/targets/{targetName}
        [HttpGet("{targetName}")]
        public async Task<IActionResult> Get(string targetName, CancellationToken cancellationToken)
        {
            var target = await this.targetService.GetAllQueryableByCriteria(t => t.Name == targetName, cancellationToken)
                .Include(t => t.RootDomains)
                .FirstOrDefaultAsync(cancellationToken);

            if (target == null)
            {
                return NotFound();
            }

            return Ok(this.mapper.Map<Target, TargetDto>(target));
        }

        // GET api/targets/{targetName}/{rootDomain}
        [HttpGet("{targetName}/{rootDomain}")]
        public async Task<IActionResult> Get(string targetName, string rootDomain, CancellationToken cancellationToken)
        {
            var target = await this.targetService.GetByCriteriaAsync(t => t.Name == targetName, cancellationToken);
            if (target == null)
            {
                return NotFound();
            }

            target.RootDomains = new List<RootDomain> { await rootDomainService.GetDomainWithSubdomainsAsync(r => r.Name == rootDomain && r.Target == target, cancellationToken) };

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

            if (await this.targetService.AnyAsync(t => t.Name.ToLower() == targetDto.Name.ToLower()))
            {
                return BadRequest("There is a Target with that name in the DB");
            }

            var target = this.mapper.Map<TargetDto, Target>(targetDto);

            await this.targetService.AddAsync(target, cancellationToken);

            return NoContent();
        }

        // PUT api/targets/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] TargetDto targetDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var target = await this.targetService.GetAllQueryableByCriteria(t => t.Id == id, cancellationToken)
                .Include(a => a.RootDomains)
                .FirstOrDefaultAsync();

            if (target == null)
            {
                return NotFound();
            }

            if (target.Name != targetDto.Name && await this.targetService.AnyAsync(t => t.Name == targetDto.Name))
            {
                return BadRequest("There is a Target with that name in the DB");
            }

            target.Name = targetDto.Name;
            target.RootDomains = this.rootDomainService.GetRootDomains(target.RootDomains, targetDto.RootDomains.Select(l => l.Name).ToList(), cancellationToken);
            target.BugBountyProgramUrl = targetDto.BugBountyProgramUrl;
            target.IsPrivate = targetDto.IsPrivate;
            target.InScope = targetDto.InScope;
            target.OutOfScope = targetDto.OutOfScope;

            await this.targetService.UpdateAsync(target, cancellationToken);

            return NoContent();
        }

        // DELETE api/targets/{targetName}
        [HttpDelete("{targetName}")]
        public async Task<IActionResult> Delete(string targetName, CancellationToken cancellationToken)
        {
            var target = await this.targetService.GetAllQueryableByCriteria(t => t.Name == targetName, cancellationToken)
                .Include(a => a.RootDomains)
                    .ThenInclude(a => a.Subdomains)
                        .ThenInclude(s => s.Services)
                .Include(a => a.RootDomains)
                    .ThenInclude(a => a.Subdomains)
                        .ThenInclude(s => s.Notes)
                .Include(a => a.RootDomains)
                    .ThenInclude(a => a.Notes)
                .FirstOrDefaultAsync(cancellationToken);

            if (target == null)
            {
                return NotFound();
            }

            await this.targetService.DeleteTargetAsync(target, cancellationToken);

            return NoContent();
        }

        // DELETE api/targets/{targetName}/{rootDomain}
        [HttpDelete("{targetName}/{rootDomain}")]
        public async Task<IActionResult> DeleteAllSubdomains(string targetName, string rootDomain, CancellationToken cancellationToken)
        {
            var target = await this.targetService.GetByCriteriaAsync(t => t.Name == targetName, cancellationToken);
            if (target == null)
            {
                return NotFound();
            }

            var domain = await this.rootDomainService.GetDomainWithSubdomainsAsync(t => t.Name == rootDomain && t.Target == target, cancellationToken);
            if (domain == null)
            {
                return NotFound();
            }

            await this.rootDomainService.DeleteSubdomainsAsync(domain, cancellationToken);

            return NoContent();
        }

        // POST api/targets/{targetName}/{rootDomain}
        [HttpPost("{targetName}/{rootDomain}")]
        public async Task<IActionResult> UploadSubdomains(string targetName, string rootDomain, IFormFile file, CancellationToken cancellationToken)
        {
            if (file.Length == 0)
            {
                return BadRequest();
            }

            var target = await this.targetService.GetByCriteriaAsync(t => t.Name == targetName, cancellationToken);
            if (target == null)
            {
                return NotFound();
            }

            var domain = await this.rootDomainService.GetDomainWithSubdomainsAsync(t => t.Name == rootDomain && t.Target == target, cancellationToken);
            if (domain == null)
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
                var subdomainsAdded = await this.rootDomainService.UploadSubdomainsAsync(domain, subdomains);

                return Ok(this.mapper.Map<List<Subdomain>, List<SubdomainDto>>(subdomainsAdded));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
