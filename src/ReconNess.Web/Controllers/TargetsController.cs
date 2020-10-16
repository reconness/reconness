using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReconNess.Core.Services;
using ReconNess.Entities;
using ReconNess.Web.Dtos;
using System;
using System.Collections.Generic;
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
        private static readonly string ERROR_TARGET_EXIT = "A target with that name exist";

        private readonly IMapper mapper;
        private readonly ITargetService targetService;
        private readonly IRootDomainService rootDomainService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetsController" /> class
        /// </summary>
        /// <param name="mapper"><see cref="IMapper"/></param>
        /// <param name="targetService"><see cref="ITargetService"/></param>
        /// <param name="rootDomainService"><see cref="IRootDomainService"/></param>
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
            var targets = await this.targetService.GetAllWithRootDomainsAsync(t => !t.Deleted, cancellationToken);

            return Ok(this.mapper.Map<List<Target>, List<TargetDto>>(targets));
        }

        // GET api/targets/{targetName}
        [HttpGet("{targetName}")]
        public async Task<IActionResult> Get(string targetName, CancellationToken cancellationToken)
        {
            var target = await this.targetService.GetWithRootDomainAsync(t => t.Name == targetName, cancellationToken);
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
            var targetExist = await this.targetService.AnyAsync(t => t.Name.ToLower() == targetDto.Name.ToLower());
            if (targetExist)
            {
                return BadRequest(ERROR_TARGET_EXIT);
            }

            var target = this.mapper.Map<TargetDto, Target>(targetDto);

            await this.targetService.AddAsync(target, cancellationToken);

            return NoContent();
        }

        // PUT api/targets/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] TargetDto targetDto, CancellationToken cancellationToken)
        {
            var target = await this.targetService.GetWithRootDomainAsync(t => t.Id == id, cancellationToken);
            if (target == null)
            {
                return NotFound();
            }

            if (target.Name != targetDto.Name && await this.targetService.AnyAsync(t => t.Name == targetDto.Name))
            {
                return BadRequest(ERROR_TARGET_EXIT);
            }

            target.Name = targetDto.Name;
            target.BugBountyProgramUrl = targetDto.BugBountyProgramUrl;
            target.IsPrivate = targetDto.IsPrivate;
            target.InScope = targetDto.InScope;
            target.OutOfScope = targetDto.OutOfScope;

            target.RootDomains = this.rootDomainService.GetRootDomains(target.RootDomains, targetDto.RootDomains.Select(l => l.Name).ToList(), cancellationToken);

            await this.targetService.UpdateAsync(target, cancellationToken);

            return NoContent();
        }

        // DELETE api/targets/{targetName}
        [HttpDelete("{targetName}")]
        public async Task<IActionResult> Delete(string targetName, CancellationToken cancellationToken)
        {
            var target = await this.targetService.GetByCriteriaAsync(t => t.Name == targetName, cancellationToken);
            if (target == null)
            {
                return NotFound();
            }

            await this.targetService.DeleteAsync(target, cancellationToken);

            return NoContent();
        }
    }
}
