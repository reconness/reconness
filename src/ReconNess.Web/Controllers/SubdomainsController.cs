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
    public class SubdomainsController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ISubdomainService subdomainService;
        private readonly IRootDomainService rootDomainService;
        private readonly ITargetService targetService;
        private readonly ILabelService labelService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubdomainsController" /> class
        /// </summary>
        /// <param name="mapper"><see cref="IMapper"/></param>
        /// <param name="subdomainService"><see cref="ISubdomainService"/></param>
        /// <param name="rootDomainService"><see cref="IRootDomainService"/></param>
        /// <param name="targetService"><see cref="ITargetService"/></param>
        /// <param name="labelService"><see cref="ILabelService"/></param>
        public SubdomainsController(
            IMapper mapper,
            ISubdomainService subdomainService,
            IRootDomainService rootDomainService,
            ITargetService targetService,
            ILabelService labelService)
        {
            this.mapper = mapper;
            this.subdomainService = subdomainService;
            this.rootDomainService = rootDomainService;
            this.targetService = targetService;
            this.labelService = labelService;
        }

        // GET api/subdomains/{target}/{rootDomainName}/{subdomain}
        [HttpGet("{targetName}/{rootDomainName}/{subdomainName}")]
        public async Task<IActionResult> Get(string targetName, string rootDomainName, string subdomainName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(targetName) || string.IsNullOrEmpty(rootDomainName) || string.IsNullOrEmpty(subdomainName))
            {
                return BadRequest();
            }

            var target = await this.targetService.GetByCriteriaAsync(t => t.Name == targetName, cancellationToken);
            if (target == null)
            {
                return BadRequest();
            }

            var rootDomain = await this.rootDomainService.GetByCriteriaAsync(t => t.Name == rootDomainName, cancellationToken);
            if (rootDomain == null)
            {
                return BadRequest();
            }

            var subdomain = await this.subdomainService.GetWithIncludeAsync(s => s.RootDomain == rootDomain && s.Name == subdomainName, cancellationToken);
            if (subdomain == null)
            {
                return NotFound();
            }

            return Ok(this.mapper.Map<Subdomain, SubdomainDto>(subdomain));
        }

        // GET api/subdomains/{target}/{rootDomainName}
        [HttpGet("GetPaginate/{targetName}/{rootDomainName}")]
        public async Task<IActionResult> GetPaginate(string targetName, string rootDomainName, [FromQuery] SubdomainQueryDto subdomainQueryDto, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(targetName) || string.IsNullOrEmpty(rootDomainName))
            {
                return BadRequest();
            }

            var target = await this.targetService.GetByCriteriaAsync(t => t.Name == targetName, cancellationToken);
            if (target == null)
            {
                return BadRequest();
            }

            var rootDomain = await this.rootDomainService.GetByCriteriaAsync(r => r.Target == target && r.Name == rootDomainName, cancellationToken);
            if (rootDomain == null)
            {
                return BadRequest();
            }

            var subdomainResult = await this.subdomainService.GetPaginateAsync(rootDomain, subdomainQueryDto.Query, subdomainQueryDto.Page, subdomainQueryDto.Limit, cancellationToken);

            var subdomains = this.mapper.Map<IList<Subdomain>, IList<SubdomainDto>>(subdomainResult.Results);

            return Ok(new { Count = subdomainResult.RowCount, Data = subdomains });
        }

        // POST api/subdomains
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SubdomainDto subdomainDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var target = await this.targetService.GetByCriteriaAsync(t => t.Name == subdomainDto.Target, cancellationToken);
            if (target == null)
            {
                return BadRequest();
            }

            var rootDomain = await this.rootDomainService.GetByCriteriaAsync(r => r.Target == target && r.Name == subdomainDto.RootDomain, cancellationToken);
            if (rootDomain == null)
            {
                return BadRequest();
            }

            var subdomainExist = await this.subdomainService.AnyAsync(s => s.Name == subdomainDto.Name && s.RootDomain == rootDomain);
            if (subdomainExist)
            {
                return BadRequest($"The subdomain {subdomainDto.Name} exist");
            }

            var newSubdoamin = await this.subdomainService.AddAsync(new Subdomain
            {
                RootDomain = rootDomain,
                Name = subdomainDto.Name
            }, cancellationToken);

            return Ok(mapper.Map<Subdomain, SubdomainDto>(newSubdoamin));
        }

        // PUT api/subdomains/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] SubdomainDto subdomainDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var subdomain = await this.subdomainService.GetWithLabelsAsync(a => a.Id == id, cancellationToken);
            if (subdomain == null)
            {
                return NotFound();
            }

            var editedSubdmain = this.mapper.Map<SubdomainDto, Subdomain>(subdomainDto);

            subdomain.Name = editedSubdmain.Name;
            subdomain.IsMainPortal = editedSubdmain.IsMainPortal;
            subdomain.Labels = await this.labelService.GetLabelsAsync(subdomain.Labels, subdomainDto.Labels.Select(l => l.Name).ToList(), cancellationToken);

            await this.subdomainService.UpdateAsync(subdomain, cancellationToken);

            return NoContent();
        }

        // PUT api/subdomains/label/{id}
        [HttpPut("label/{id}")]
        public async Task<IActionResult> AddLabel(Guid id, [FromBody] SubdomainLabelDto subdomainLabelDto, CancellationToken cancellationToken)
        {
            var subdomain = await this.subdomainService.GetWithLabelsAsync(a => a.Id == id, cancellationToken);
            if (subdomain == null)
            {
                return NotFound();
            }

            await this.subdomainService.AddLabelAsync(subdomain, subdomainLabelDto.Label, cancellationToken);

            return NoContent();
        }

        // DELETE api/subdomains/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var subdomain = await this.subdomainService.GetByCriteriaAsync(a => a.Id == id, cancellationToken);
            if (subdomain == null)
            {
                return NotFound();
            }

            await this.subdomainService.DeleteAsync(subdomain, cancellationToken);

            return NoContent();
        }
    }
}
