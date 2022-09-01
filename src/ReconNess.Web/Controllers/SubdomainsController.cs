using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReconNess.Core.Services;
using ReconNess.Entities;
using ReconNess.Helpers;
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
        private readonly IEventTrackService eventTrackService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubdomainsController" /> class
        /// </summary>
        /// <param name="mapper"><see cref="IMapper"/></param>
        /// <param name="subdomainService"><see cref="ISubdomainService"/></param>
        /// <param name="rootDomainService"><see cref="IRootDomainService"/></param>
        /// <param name="targetService"><see cref="ITargetService"/></param>
        /// <param name="labelService"><see cref="ILabelService"/></param>
        /// <param name="eventTrackService"><see cref="IEventTrackService"/></param>
        public SubdomainsController(
            IMapper mapper,
            ISubdomainService subdomainService,
            IRootDomainService rootDomainService,
            ITargetService targetService,
            ILabelService labelService,
            IEventTrackService eventTrackService)
        {
            this.mapper = mapper;
            this.subdomainService = subdomainService;
            this.rootDomainService = rootDomainService;
            this.targetService = targetService;
            this.labelService = labelService;
            this.eventTrackService = eventTrackService;
        }

        /// <summary>
        /// Obtain a subdomain.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET api/subdomains/{target}/{rootDomainName}/{subdomain}
        ///
        /// </remarks>
        /// <param name="targetName">The target name</param>
        /// <param name="rootDomainName">The rootdomain</param>
        /// <param name="subdomainName">The subdomain</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A subdomain</returns>
        /// <response code="200">Returns a subdomain</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">If the user is not authenticate</response>
        [HttpGet("{targetName}/{rootDomainName}/{subdomainName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Get([FromRoute] string targetName, [FromRoute] string rootDomainName, [FromRoute] string subdomainName, CancellationToken cancellationToken)
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
        /// Obtain the list of subdomains paginate.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET api/subdomains/{target}/{rootDomainName}
        ///     {
        ///         "query": "443",
        ///         "limit": 10,
        ///         "ascending": 1,
        ///         "page", 1,
        ///         "byColumn": 1
        ///     }
        ///
        /// </remarks>
        /// <param name="targetName">The target name</param>
        /// <param name="rootDomainName">The rootdomain</param>
        /// <param name="subdomainQueryDto">The subdomain query dto</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The list of subdomains paginate</returns>
        /// <response code="200">Returns the list of subdomains paginate</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">If the user is not authenticate</response>
        [HttpGet("GetPaginate/{targetName}/{rootDomainName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetPaginate([FromRoute] string targetName, [FromRoute] string rootDomainName, [FromQuery] SubdomainQueryDto subdomainQueryDto, CancellationToken cancellationToken)
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
            
            foreach (var subdomain in subdomains)
            {
                subdomain.Screenshot = SubdomainHelpers.GetBase64Image(targetName, rootDomainName, subdomain.Name);
            }

            return Ok(new { Count = subdomainResult.RowCount, Data = subdomains });
        }       

        /// <summary>
        /// Save a new subdomain.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/subdomains
        ///     {
        ///         "name": "www.xxxxx.com",
        ///         "target": "xxxxx",
        ///         "rootDomain": "xxxxx.com"
        ///     }
        ///
        /// </remarks>
        /// <param name="subdomainDto">The subdomain dto</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A new subdomain</returns>
        /// <response code="200">Returns a new subdomain</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">If the user is not authenticate</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Post([FromBody] SubdomainCreateDto subdomainDto, CancellationToken cancellationToken)
        {
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

            var subdomainExist = await this.subdomainService.AnyAsync(s => s.RootDomain == rootDomain && s.Name == subdomainDto.Name, cancellationToken);
            if (subdomainExist)
            {
                return BadRequest($"The subdomain {subdomainDto.Name} exist");
            }

            var newSubdoamin = await this.subdomainService.AddAsync(new Subdomain
            {
                RootDomain = rootDomain,
                Name = subdomainDto.Name
            }, cancellationToken);

            await this.eventTrackService.AddAsync(new EventTrack
            {
                Target = target,
                RootDomain = rootDomain,
                Subdomain = newSubdoamin,
                Data = $"Subdomain {newSubdoamin.Name} added"
            }, cancellationToken);

            return Ok(mapper.Map<Subdomain, SubdomainDto>(newSubdoamin));
        }

        /// <summary>
        /// Save multiples subdomains.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/subdomains
        ///     {
        ///         [
        ///             {
        ///                 "name": "www.xxxxx.com",
        ///                 "target": "xxxxx",
        ///                 "rootDomain": "xxxxx.com"
        ///             },
        ///             {
        ///                 "name": "www1.xxxxx.com",
        ///                 "target": "xxxxx",
        ///                 "rootDomain": "xxxxx.com"
        ///             },
        ///         ]
        ///     }
        ///
        /// </remarks>
        /// <param name="subdomainDtos">The subdomains dto</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A new subdomain</returns>
        /// <response code="200">Returns a new subdomain</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">If the user is not authenticate</response>
        [HttpPost("Multiples")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Multiples([FromBody] IList<SubdomainCreateDto> subdomainDtos, CancellationToken cancellationToken)
        {
            if (subdomainDtos == null || subdomainDtos.Count == 0)
            {
                return BadRequest("You need to send at least one new subdomain to save.");
            }

            var firstSubdomain = subdomainDtos.First();
            var target = await this.targetService.GetByCriteriaAsync(t => t.Name == firstSubdomain.Target, cancellationToken);
            if (target == null)
            {
                return BadRequest($"The target {firstSubdomain.Target} does not exist.");
            }

            var rootDomain = await this.rootDomainService.GetByCriteriaAsync(r => r.Target == target && r.Name == firstSubdomain.RootDomain, cancellationToken);
            if (rootDomain == null)
            {
                return BadRequest($"The root domain {firstSubdomain.RootDomain} does not exist.");
            }

            var newSubdomainDtos = new List<SubdomainDto>();
            foreach (var subdomainDto in subdomainDtos)
            {
                var subdomainExist = await this.subdomainService.AnyAsync(s => s.RootDomain == rootDomain && s.Name == subdomainDto.Name, cancellationToken);
                if (subdomainExist)
                {
                    continue;
                }

                var newSubdomain = await this.subdomainService.AddAsync(new Subdomain
                {
                    RootDomain = rootDomain,
                    Name = subdomainDto.Name
                }, cancellationToken);

                await this.eventTrackService.AddAsync(new EventTrack
                {
                    Target = target,
                    RootDomain = rootDomain,
                    Subdomain = newSubdomain,
                    Data = $"Subdomain {newSubdomain.Name} added"
                }, cancellationToken);

                newSubdomainDtos.Add(mapper.Map<Subdomain, SubdomainDto>(newSubdomain));
                
            }

            if (newSubdomainDtos.Count == 0)
            {
                return BadRequest($"We did not save any new subdomain.");
            }

            return Ok(newSubdomainDtos);
        }

        /// <summary>
        /// Update a subdomain.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT api/subdomains/{id}
        ///     {
        ///         "name": "updated.xxxxx.com",
        ///         "isMainPortal": true,
        ///         "labels":
        ///         [
        ///             {
        ///                 "name": "bug"
        ///             },
        ///             {
        ///                 "name": "bounty"
        ///             }
        ///         ]
        ///     }
        ///
        /// </remarks>
        /// <param name="id">The subdomain id</param>
        /// <param name="subdomainDto">The subdomain dto</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <response code="204">No Content</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">If the user is not authenticate</response>
        /// <response code="404">Not Found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] SubdomainDto subdomainDto, CancellationToken cancellationToken)
        {
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

            await this.eventTrackService.AddAsync(new EventTrack
            {
                Target = target,
                RootDomain = rootDomain,
                Subdomain = subdomain,
                Data = $"Subdomain {subdomain.Name} updated"
            }, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// Add a new label to the subdomian.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT api/subdomains/label/{id}
        ///     {
        ///         "label": "newlabel"
        ///     }
        ///
        /// </remarks>
        /// <param name="id">The subdomain id</param>
        /// <param name="subdomainLabelDto">The subdomain label dto</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <response code="204">No Content</response>
        /// <response code="401">If the user is not authenticate</response>
        /// <response code="404">Not Found</response>
        [HttpPut("label/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddLabel([FromRoute] Guid id, [FromBody] SubdomainLabelDto subdomainLabelDto, CancellationToken cancellationToken)
        {
            var subdomain = await this.subdomainService.GetWithLabelsAsync(a => a.Id == id, cancellationToken);
            if (subdomain == null)
            {
                return NotFound();
            }

            await this.subdomainService.AddLabelAsync(subdomain, subdomainLabelDto.Label, cancellationToken);

            await this.eventTrackService.AddAsync(new EventTrack
            {
                Target = subdomain.RootDomain.Target,
                RootDomain = subdomain.RootDomain,
                Subdomain = subdomain,
                Data = $"Subdomain {subdomain.Name} Label {subdomainLabelDto.Label} added"
            }, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// Delete a subdomain.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE api/subdomains/{id}
        ///
        /// </remarks>
        /// <param name="id">The subdomain id</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <response code="204">No Content</response>
        /// <response code="401">If the user is not authenticate</response>
        /// <response code="404">Not Found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var subdomain = await this.subdomainService.GetSubdomainAsync(a => a.Id == id, cancellationToken);
            if (subdomain == null)
            {
                return NotFound();
            }

            await this.subdomainService.DeleteAsync(subdomain, cancellationToken);

            await this.eventTrackService.AddAsync(new EventTrack
            {
                Target = subdomain.RootDomain.Target,
                RootDomain = subdomain.RootDomain,
                Data = $"Subdomain {subdomain.Name} deleted"
            }, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// Delete multiples subdomains.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE api/subdomains/{targetName}/{rootDomainName}/multiples
        /// {
        ///     [
        ///         "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
        ///         "yyyyyyyy-yyyy-yyyy-yyyy-yyyyyyyyyyyy"
        ///     }
        ///
        /// </remarks>
        /// <param name="targetName">The target name</param>
        /// <param name="rootDomainName">The rootdomain name</param>
        /// <param name="subdomainIds">The subdomains guid list to delete</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <response code="204">No Content</response>
        /// <response code="401">If the user is not authenticate</response>
        /// <response code="404">Not Found</response>
        [HttpDelete("{targetName}/{rootDomainName}/Multiples")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMultiples([FromRoute] string targetName, [FromRoute] string rootDomainName, [FromBody] IList<Guid> subdomainIds, CancellationToken cancellationToken)
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

            if (subdomainIds == null || subdomainIds.Count == 0)
            {
                return BadRequest("You need to send at least one subdomain to delete.");
            }

            foreach (var subdomainId in subdomainIds)
            {
                var subdomain = await this.subdomainService.GetSubdomainAsync(s => s.Id == subdomainId && s.RootDomain == rootDomain && s.RootDomain.Target == target, cancellationToken);
                if (subdomain == null)
                {
                    return NotFound();
                }

                await this.subdomainService.DeleteAsync(subdomain, cancellationToken);

                await this.eventTrackService.AddAsync(new EventTrack
                {
                    Target = subdomain.RootDomain.Target,
                    RootDomain = subdomain.RootDomain,
                    Data = $"Subdomain {subdomain.Name} deleted"
                }, cancellationToken);
                
            }

            return NoContent();
        }
    }

}
