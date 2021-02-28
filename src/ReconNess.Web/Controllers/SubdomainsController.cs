using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    [Produces("application/json")]
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

        /// <summary>
        /// Obtain the notifications configuration.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET api/subdomains/{target}/{rootDomainName}/{subdomain}
        ///
        /// </remarks>
        /// <param name="targetName"></param>
        /// <param name="rootDomainName"></param>
        /// <param name="subdomainName"></param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The notifications configuration</returns>
        /// <response code="200">Returns the notifications configuration</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">If the user is not authenticate</response>
        [HttpGet("{targetName}/{rootDomainName}/{subdomainName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Get(string targetName, string rootDomainName, string subdomainName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(targetName) || string.IsNullOrEmpty(rootDomainName) || string.IsNullOrEmpty(subdomainName))
            {
                return BadRequest();
            }

            var target = await this.targetService.GetTargetNotTrackingAsync(t => t.Name == targetName, cancellationToken);
            if (target == null)
            {
                return BadRequest();
            }

            var rootDomain = await this.rootDomainService.GetRootDomainNoTrackingAsync(r => r.Target == target && r.Name == rootDomainName, cancellationToken);
            if (rootDomain == null)
            {
                return BadRequest();
            }

            var subdomain = await this.subdomainService.GetSubdomainNoTrackingAsync(s => s.RootDomain == rootDomain && s.Name == subdomainName, cancellationToken);
            if (subdomain == null)
            {
                return NotFound();
            }

            return Ok(this.mapper.Map<Subdomain, SubdomainDto>(subdomain));
        }

        /// <summary>
        /// Obtain the notifications configuration.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET api/subdomains/{target}/{rootDomainName}
        ///
        /// </remarks>
        /// <param name="targetName"></param>
        /// <param name="rootDomainName"></param>
        /// <param name="subdomainQueryDto"></param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The notifications configuration</returns>
        /// <response code="200">Returns the notifications configuration</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">If the user is not authenticate</response>
        [HttpGet("GetPaginate/{targetName}/{rootDomainName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetPaginate(string targetName, string rootDomainName, [FromQuery] SubdomainQueryDto subdomainQueryDto, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(targetName) || string.IsNullOrEmpty(rootDomainName))
            {
                return BadRequest();
            }

            var target = await this.targetService.GetTargetNotTrackingAsync(t => t.Name == targetName, cancellationToken);
            if (target == null)
            {
                return BadRequest();
            }

            var rootDomain = await this.rootDomainService.GetRootDomainNoTrackingAsync(r => r.Target == target && r.Name == rootDomainName, cancellationToken);
            if (rootDomain == null)
            {
                return BadRequest();
            }

            var subdomainResult = await this.subdomainService.GetPaginateAsync(rootDomain, subdomainQueryDto.Query, subdomainQueryDto.Page, subdomainQueryDto.Limit, cancellationToken);

            var subdomains = this.mapper.Map<IList<Subdomain>, IList<SubdomainDto>>(subdomainResult.Results);

            return Ok(new { Count = subdomainResult.RowCount, Data = subdomains });
        }

        /// <summary>
        /// Obtain the notifications configuration.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/subdomains
        ///
        /// </remarks>
        /// <param name="subdomainDto"></param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The notifications configuration</returns>
        /// <response code="200">Returns the notifications configuration</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">If the user is not authenticate</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Post([FromBody] SubdomainDto subdomainDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var target = await this.targetService.GetTargetNotTrackingAsync(t => t.Name == subdomainDto.Target, cancellationToken);
            if (target == null)
            {
                return BadRequest();
            }

            var rootDomain = await this.rootDomainService.GetRootDomainNoTrackingAsync(r => r.Target == target && r.Name == subdomainDto.RootDomain, cancellationToken);
            if (rootDomain == null)
            {
                return BadRequest();
            }

            var subdomainExist = await this.subdomainService.AnyAsync(s => s.RootDomain == rootDomain && s.Name == subdomainDto.Name);
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

        /// <summary>
        /// Obtain the notifications configuration.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT api/subdomains/{id}
        ///
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="subdomainDto"></param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The notifications configuration</returns>
        /// <response code="204">No Content</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">If the user is not authenticate</response>
        /// <response code="404">Not Found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]        
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Obtain the notifications configuration.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT api/subdomains/label/{id}
        ///
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="subdomainLabelDto"></param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The notifications configuration</returns>
        /// <response code="204">No Content</response>
        /// <response code="401">If the user is not authenticate</response>
        /// <response code="404">Not Found</response>
        [HttpPut("label/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]        
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Obtain the notifications configuration.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE api/subdomains/{id}
        ///
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The notifications configuration</returns>
        /// <response code="204">No Content</response>
        /// <response code="401">If the user is not authenticate</response>
        /// <response code="404">Not Found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]        
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
