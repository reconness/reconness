using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ReconNess.Core.Services;
using ReconNess.Entities;
using ReconNess.Web.Dtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Web.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class RootDomainsController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ITargetService targetService;
        private readonly IRootDomainService rootDomainService;
        private readonly IEventTrackService eventTrackService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetsController" /> class
        /// </summary>
        /// <param name="mapper"><see cref="IMapper"/></param>
        /// <param name="targetService"><see cref="ITargetService"/></param>
        /// <param name="rootDomainService"><see cref="IRootDomainService"/></param>
        /// <param name="eventTrackService"><see cref="IEventTrackService"/></param>
        public RootDomainsController(
            IMapper mapper,
            ITargetService targetService,
            IRootDomainService rootDomainService,
            IEventTrackService eventTrackService)
        {
            this.mapper = mapper;
            this.targetService = targetService;
            this.rootDomainService = rootDomainService;
            this.eventTrackService = eventTrackService;
        }

        /// <summary>
        /// Obtain a rootdomain.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET api/rootdomains/{targetName}/{rootDomainName}
        ///
        /// </remarks>
        /// <param name="targetName">The target name</param>
        /// <param name="rootDomainName">The rootdomain</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A rootdomain</returns>
        /// <response code="200">Returns a rootdomain</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">If the user is not authenticate</response>
        [HttpGet("{targetName}/{rootDomainName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Get([FromRoute] string targetName, [FromRoute] string rootDomainName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(targetName) || string.IsNullOrEmpty(rootDomainName))
            {
                return BadRequest();
            }

            var target = await targetService.GetTargetNotTrackingAsync(t => t.Name == targetName, cancellationToken);
            if (target == null)
            {
                return NotFound();
            }

            var rootDomain = await rootDomainService.GetRootDomainNoTrackingAsync(r => r.Target == target && r.Name == rootDomainName, cancellationToken);
            if (rootDomain == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<RootDomain, RootDomainDto>(rootDomain));
        }

        /// <summary>
        /// Delete a rootdomain.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE api/rootdomains/{targetName}/{rootDomainName}
        ///
        /// </remarks>
        /// <param name="targetName">The target name</param>
        /// <param name="rootDomainName">The rootdomain to delete</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <response code="204">No Content</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">If the user is not authenticate</response>
        /// <response code="404">Not Found</response>
        [HttpDelete("{targetName}/{rootDomainName}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] string targetName, [FromRoute] string rootDomainName, CancellationToken cancellationToken)
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

            var rootDomain = await this.rootDomainService.GetRootDomainWithSubdomainsAsync(r => r.Target == target && r.Name == rootDomainName, cancellationToken);
            if (rootDomain == null)
            {
                return NotFound();
            }

            await this.rootDomainService.DeleteAsync(rootDomain, cancellationToken);

            await this.eventTrackService.AddAsync(new EventTrack
            {
                Target = target,
                Data = $"Rootdomain {rootDomain.Name} deleted"
            }, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// Delete all the subdomains belong to the rootdomain.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE api/rootdomains/deleteSubdomians/{targetName}/{rootDomainName}
        ///
        /// </remarks>
        /// <param name="targetName">The target name</param>
        /// <param name="rootDomainName">The rootdomain</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <response code="204">No Content</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">If the user is not authenticate</response>
        /// <response code="404">Not Found</response>
        [HttpDelete("deleteSubdomians/{targetName}/{rootDomainName}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteSubdomains([FromRoute] string targetName, [FromRoute] string rootDomainName, CancellationToken cancellationToken)
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

            var rootDomain = await rootDomainService.GetRootDomainWithSubdomainsAsync(r => r.Target == target && r.Name == rootDomainName, cancellationToken);
            if (rootDomain == null)
            {
                return NotFound();
            }

            await rootDomainService.DeleteSubdomainsAsync(rootDomain, cancellationToken);

            await this.eventTrackService.AddAsync(new EventTrack
            {
                Target = target,
                RootDomain = rootDomain,
                Data = $"Rootdomain {rootDomain.Name} deleted subdomains"
            }, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// Import a rootdomain with all the subdomains in the target.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/rootdomains/import/{targetName}
        ///
        /// </remarks>
        /// <param name="targetName">The target name</param>
        /// <param name="file">the rootdomain json with all the subdomains too</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The rootdomain name imported</returns>
        /// <response code="200">Returns the rootdomain name imported</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">If the user is not authenticate</response>
        /// <response code="404">Not Found</response>
        [HttpPost("import/{targetName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Import([FromRoute] string targetName, IFormFile file, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(targetName))
            {
                return BadRequest();
            }

            if (file.Length == 0)
            {
                return BadRequest();
            }

            var target = await this.targetService.GetTargetAsync(t => t.Name == targetName, cancellationToken);
            if (target == null)
            {
                return NotFound();
            }

            var path = Path.GetTempFileName();
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            RootDomainDto rootDomainDto = default;
            try
            {
                var json = System.IO.File.ReadAllLines(path).FirstOrDefault();
                rootDomainDto = JsonConvert.DeserializeObject<RootDomainDto>(json);
            }
            catch (Exception)
            {
                return BadRequest("Invalid rootdomain json");
            }

            if (string.IsNullOrEmpty(rootDomainDto.Name))
            {
                return BadRequest($"The rootdomain name can not be empty");
            }

            if (target.RootDomains.Any(r => rootDomainDto.Name.Equals(r.Name, StringComparison.OrdinalIgnoreCase)))
            {
                return BadRequest($"The rootdomain: {rootDomainDto.Name} exist in the target");
            }

            var uploadRootDomain = this.mapper.Map<RootDomainDto, RootDomain>(rootDomainDto);

            uploadRootDomain.Target = target;
            var uploadedRootDomain = await this.rootDomainService.ImportRootDomainAsync(uploadRootDomain, cancellationToken);

            await this.eventTrackService.AddAsync(new EventTrack
            {
                Target = target,
                RootDomain = uploadedRootDomain,
                Data = $"Rootdomain {uploadRootDomain.Name} imported"
            }, cancellationToken);

            var uploadRootDomainDto = this.mapper.Map<RootDomain, RootDomainDto>(uploadedRootDomain);

            return Ok(uploadRootDomainDto);
        }

        /// <summary>
        /// Export the rootdomian with all the subdomains.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET api/rootdomains/export/{targetName}/{rootDomainName}
        ///
        /// </remarks>
        /// <param name="targetName">The target name</param>
        /// <param name="rootDomainName">The rootdomain</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The json file with the rootdomain and the subdomains data</returns>
        /// <response code="200">Returns the json file with the rootdomain and the subdomains data</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">If the user is not authenticate</response>
        /// <response code="404">Not Found</response>
        [HttpPost("export/{targetName}/{rootDomainName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Export([FromRoute] string targetName, [FromRoute] string rootDomainName, CancellationToken cancellationToken)
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

            var exportRootDomain = await this.rootDomainService.ExportRootDomainAsync(r => r.Target == target && r.Name == rootDomainName, cancellationToken);

            var rootdomainDto = this.mapper.Map<RootDomain, RootDomainDto>(exportRootDomain);

            var download = Helpers.ZipSerializedObject<RootDomainDto>(rootdomainDto);

            await this.eventTrackService.AddAsync(new EventTrack
            {
                Target = target,
                RootDomain = rootDomain,
                Data = $"Rootdomain {rootDomain.Name} exported"
            }, cancellationToken);

            return File(download, "application/json", $"rootdomain-{rootDomain.Name}.json");
        }

        /// <summary>
        /// Upload a list of subdomains to the rootdomain.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/rootdomains/uploadSubdomains/{targetName}/{rootDomainName}
        ///
        /// </remarks>
        /// <param name="targetName">The target name</param>
        /// <param name="rootDomainName">The rootdomain</param>
        /// <param name="file">The file with all the subdomains to be uploaded</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <response code="204">No Content</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">If the user is not authenticate</response>
        /// <response code="404">Not Found</response>
        [HttpPost("uploadSubdomains/{targetName}/{rootDomainName}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UploadSubdomains([FromRoute] string targetName, [FromRoute] string rootDomainName, IFormFile file, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(targetName) || string.IsNullOrEmpty(rootDomainName))
            {
                return BadRequest();
            }

            if (file.Length == 0)
            {
                return BadRequest();
            }

            var target = await this.targetService.GetByCriteriaAsync(t => t.Name == targetName, cancellationToken);
            if (target == null)
            {
                return NotFound();
            }

            var rootDomain = await rootDomainService.GetRootDomainWithSubdomainsAsync(r => r.Target == target && r.Name == rootDomainName, cancellationToken);
            if (rootDomain == null)
            {
                return NotFound();
            }

            var path = Path.GetTempFileName();
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            var subdomains = System.IO.File.ReadAllLines(path).ToList();
            await rootDomainService.UploadSubdomainsAsync(rootDomain, subdomains, cancellationToken);

            await this.eventTrackService.AddAsync(new EventTrack
            {
                Target = target,
                RootDomain = rootDomain,
                Data = $"Rootdomain {rootDomain.Name} uploaded {subdomains.Count} subdomains"
            }, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// Download all the subdomain to a csv file.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/rootdomains/exportSubdomains/{targetName}/{rootDomainName}
        ///
        /// </remarks>
        /// <param name="targetName">The target name</param>
        /// <param name="rootDomainName">The rootdomain</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The csv file with all the subdomains beloing to the rootdomain</returns>
        /// <response code="200">Returns the csv file with all the subdomains beloing to the rootdomain</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">If the user is not authenticate</response>
        /// <response code="404">Not Found</response>
        [HttpPost("exportSubdomains/{targetName}/{rootDomainName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DonwloadSubdomains([FromRoute] string targetName, [FromRoute] string rootDomainName, CancellationToken cancellationToken)
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

            var rootDomain = await rootDomainService.GetRootDomainWithSubdomainsAsync(r => r.Target == target && r.Name == rootDomainName, cancellationToken);
            if (rootDomain == null)
            {
                return NotFound();
            }

            var data = string.Join(",", rootDomain.Subdomains.Select(s => s.Name));

            var download = Encoding.UTF8.GetBytes(data);

            await this.eventTrackService.AddAsync(new EventTrack
            {
                Target = target,
                RootDomain = rootDomain,
                Data = $"Rootdomain {rootDomain.Name} downloaded {rootDomain.Subdomains.Count} subdomains"
            }, cancellationToken);

            return File(download, "text/csv", "subdomains.csv");
        }
    }
}
