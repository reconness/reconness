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
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Web.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class TargetsController : ControllerBase
    {
        private static readonly string ERROR_TARGET_EXIT = "A target with that name exist";

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
        public TargetsController(
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
        /// Obtain the list of targets.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET api/targets
        ///
        /// </remarks>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The list of targets</returns>
        /// <response code="200">Returns the list of targets</response>
        /// <response code="401">If the user is not authenticate</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var targets = await targetService.GetTargetsNotTrackingAsync(t => !t.Deleted, cancellationToken);

            return Ok(mapper.Map<List<Target>, List<TargetDto>>(targets));
        }

        /// <summary>
        /// Obtain a target by name.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET api/targets/{targetName}
        ///
        /// </remarks>
        /// <param name="targetName">The target name</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A target by name</returns>
        /// <response code="200">Returns a target by name</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">If the user is not authenticate</response>
        [HttpGet("{targetName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Get([FromRoute] string targetName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(targetName))
            {
                return BadRequest();
            }

            var target = await targetService.GetTargetNotTrackingAsync(t => t.Name == targetName, cancellationToken);
            if (target == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<Target, TargetDto>(target));
        }

        /// <summary>
        /// Save a target.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/targets
        ///     {
        ///         "name": "targetname",
        ///         "inScope": "details about what is in scope",
        ///         "outOfScope": "details about what is out of scope",
        ///         "bugBountyProgramUrl": "https://www.hackerone.com/xxxxx",
        ///         "isPrivate": "true",
        ///         "rootDomains": [{
        ///           "name": "xxxxx"  
        ///         }]
        ///     }
        ///
        /// </remarks>
        /// <param name="targetDto">The target dto</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <response code="200">Return the created target</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">If the user is not authenticate</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Post([FromBody] TargetDto targetDto, CancellationToken cancellationToken)
        {
            var targetExist = await targetService.AnyAsync(t => t.Name.ToLower() == targetDto.Name.ToLower(), cancellationToken);
            if (targetExist)
            {
                return BadRequest(ERROR_TARGET_EXIT);
            }

            var target = mapper.Map<TargetDto, Target>(targetDto);

            var insertedTarget = await this.targetService.AddAsync(target, cancellationToken);
            var insertedTargetDto = this.mapper.Map<Target, TargetDto>(insertedTarget);

            await this.eventTrackService.AddAsync(new EventTrack
            {
                Target = insertedTarget,
                Data = $"Target {insertedTarget.Name} added"
            }, cancellationToken);

            return Ok(insertedTargetDto);
        }

        /// <summary>
        /// Update a target.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT api/targets/{id}
        ///    {
        ///         "name": "targetname",
        ///         "inScope": "details about what is in scope updated",
        ///         "outOfScope": "details about what is out of scope updated",
        ///         "bugBountyProgramUrl": "https://www.hackerone.com/xxxxx",
        ///         "isPrivate": "true",
        ///         "rootDomains": [{
        ///           "name": "xxxxx"  
        ///         }]
        ///     }
        ///
        /// </remarks>
        /// <param name="id">The target id</param>
        /// <param name="targetDto">The target dto</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <response code="204">No Content</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">If the user is not authenticate</response>
        /// <response code="404">Not Found</response>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] TargetDto targetDto, CancellationToken cancellationToken)
        {
            var target = await targetService.GetTargetAsync(t => t.Id == id, cancellationToken);
            if (target == null)
            {
                return NotFound();
            }

            if (target.Name != targetDto.Name && await targetService.AnyAsync(t => t.Name == targetDto.Name, cancellationToken))
            {
                return BadRequest(ERROR_TARGET_EXIT);
            }

            target.Name = targetDto.Name;
            target.BugBountyProgramUrl = targetDto.BugBountyProgramUrl;
            target.IsPrivate = targetDto.IsPrivate;
            target.InScope = targetDto.InScope;
            target.OutOfScope = targetDto.OutOfScope;
            target.PrimaryColor = targetDto.PrimaryColor;
            target.SecondaryColor = targetDto.SecondaryColor;

            target.RootDomains = rootDomainService.GetRootDomains(target.RootDomains, targetDto.RootDomains.Select(l => l.Name).ToList(), cancellationToken);

            await targetService.UpdateAsync(target, cancellationToken);

            await this.eventTrackService.AddAsync(new EventTrack
            {
                Target = target,
                Data = $"Target {target.Name} updated"
            }, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// Delete a target.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE api/targets/{targetName}
        ///
        /// </remarks>
        /// <param name="targetName">The target name</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <response code="204">No Content</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">If the user is not authenticate</response>
        /// <response code="404">Not Found</response>
        [HttpDelete("{targetName}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] string targetName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(targetName))
            {
                return BadRequest();
            }

            var target = await targetService.GetTargetAsync(t => t.Name == targetName, cancellationToken);
            if (target == null)
            {
                return NotFound();
            }

            await targetService.DeleteAsync(target, cancellationToken);

            await this.eventTrackService.AddAsync(new EventTrack
            {
                Data = $"Target {target.Name} deleted"
            }, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// Delete a target.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE api/targets/batch
        ///     ["target1", "target2", target3]
        ///
        /// </remarks>
        /// <param name="targetNames">The target name list</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <response code="204">No Content</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">If the user is not authenticate</response>
        /// <response code="404">Not Found</response>
        [HttpDelete("batch")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteBatch([FromBody] IList<string> targetNames, CancellationToken cancellationToken)
        {
            if (targetNames == null || targetNames.Count == 0)
            {
                return BadRequest();
            }

            var targetsEntities = new List<Target>();
            foreach (var targetName in targetNames)
            {
                var target = await this.targetService.GetByCriteriaAsync(t => t.Name == targetName, cancellationToken);
                if (target != null)
                {
                    targetsEntities.Add(target);
                }   
            }

            await this.targetService.DeleteRangeAsync(targetsEntities, cancellationToken);

            foreach (var target in targetsEntities)
            {
                await this.eventTrackService.AddAsync(new EventTrack
                {
                    Data = $"Target {target.Name} deleted"
                }, cancellationToken);
            }

            return NoContent();
        }

        /// <summary>
        /// Import a target with all the rootdomain.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/targets/import
        ///
        /// </remarks>
        /// <param name="file">the rootdomain json with all the subdomains too</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The rootdomain name imported</returns>
        /// <response code="200">Returns the rootdomain name imported</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">If the user is not authenticate</response>
        /// <response code="404">Not Found</response>
        [HttpPost("import")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Import(IFormFile file, CancellationToken cancellationToken)
        {
            if (file.Length == 0)
            {
                return BadRequest();
            }

            var path = Path.GetTempFileName();
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            TargetDto targetDto = default;
            try
            {
                var json = System.IO.File.ReadAllLines(path).FirstOrDefault();
                targetDto = JsonConvert.DeserializeObject<TargetDto>(json);
            }
            catch (Exception)
            {
                return BadRequest("Invalid target json");
            }

            if (string.IsNullOrEmpty(targetDto.Name))
            {
                return BadRequest($"The target name can not be empty");
            }

            if (await this.targetService.AnyAsync(r => targetDto.Name.Equals(r.Name, StringComparison.OrdinalIgnoreCase)))
            {
                return BadRequest($"The target: {targetDto.Name} exist.");
            }

            var uploadTarget = this.mapper.Map<TargetDto, Target>(targetDto);

            var target = await this.targetService.AddAsync(uploadTarget, cancellationToken);

            await this.eventTrackService.AddAsync(new EventTrack
            {
                Target = target,
                Data = $"Target {target.Name} imported"
            }, cancellationToken);

            return Ok(uploadTarget.Name);
        }

        /// <summary>
        /// Export the target with all the rootdomains.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET api/targets/export/{targetName}
        ///
        /// </remarks>
        /// <param name="targetName">The target name</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The json file with the target and the rootdomain data</returns>
        /// <response code="200">Returns the json file with the rootdomain and the subdomains data</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">If the user is not authenticate</response>
        /// <response code="404">Not Found</response>
        [HttpPost("export/{targetName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Export([FromRoute] string targetName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(targetName))
            {
                return BadRequest();
            }

            var target = await this.targetService.GetTargetNotTrackingAsync(t => t.Name == targetName, cancellationToken);
            if (target == null)
            {
                return NotFound();
            }

            var targetDto = this.mapper.Map<Target, TargetDto>(target);

            var download = Helpers.Helpers.ZipSerializedObject<TargetDto>(targetDto);

            await this.eventTrackService.AddAsync(new EventTrack
            {
                Target = target,
                Data = $"Target {target.Name} exported"
            }, cancellationToken);

            return File(download, "application/json", $"target-{targetDto.Name}.json");
        }

        /// <summary>
        /// Obtain a target by name.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET api/targets/{targetName}
        ///
        /// </remarks>
        /// <param name="targetName">The target name</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A target by name</returns>
        /// <response code="200">Returns a target by name</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">If the user is not authenticate</response>
        [HttpGet("dashboard/{targetName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetDashboard([FromRoute] string targetName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(targetName))
            {
                return BadRequest();
            }

            var target = await this.targetService.GetTargetAsync(t => t.Name == targetName, cancellationToken);
            if (target == null)
            {
                return NotFound();
            }

            return Ok(await this.targetService.GetDashboardAsync(target, cancellationToken));
        }
    }
}
