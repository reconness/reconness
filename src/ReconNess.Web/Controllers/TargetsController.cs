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
            var targets = await this.targetService.GetTargetsNotTrackingAsync(t => !t.Deleted, cancellationToken);

            return Ok(this.mapper.Map<List<Target>, List<TargetDto>>(targets));
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
        public async Task<IActionResult> Get(string targetName, CancellationToken cancellationToken)
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

            return Ok(this.mapper.Map<Target, TargetDto>(target));
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
            var targetExist = await this.targetService.AnyAsync(t => t.Name.ToLower() == targetDto.Name.ToLower(), cancellationToken);
            if (targetExist)
            {
                return BadRequest(ERROR_TARGET_EXIT);
            }

            var target = this.mapper.Map<TargetDto, Target>(targetDto);

            var insertedTarget = await this.targetService.AddAsync(target, cancellationToken);
            var insertedTargetDto = this.mapper.Map<Target, TargetDto>(insertedTarget);

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
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put(Guid id, [FromBody] TargetDto targetDto, CancellationToken cancellationToken)
        {
            var target = await this.targetService.GetTargetAsync(t => t.Id == id, cancellationToken);
            if (target == null)
            {
                return NotFound();
            }

            if (target.Name != targetDto.Name && await this.targetService.AnyAsync(t => t.Name == targetDto.Name, cancellationToken))
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
        public async Task<IActionResult> Delete(string targetName, CancellationToken cancellationToken)
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

            await this.targetService.DeleteAsync(target, cancellationToken);

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
        public async Task<IActionResult> DeleteBatch([FromBody] List<String> targetNames, CancellationToken cancellationToken)
        {
            if (targetNames == null || targetNames.Count == 0)
            {
                return BadRequest();
            }

            List<Target> targetsEntities = new List<Target>();
            foreach (String targetName in targetNames)
            {
              Target target = await this.targetService.GetByCriteriaAsync(t => t.Name == targetName, cancellationToken);
              if (target != null)
              {
                targetsEntities.Add(target);
              }   
            }

            await this.targetService.DeleteRangeAsync(targetsEntities, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// Import a rootdomain with all the subdomains in the target.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/targets/importRootDomain/{targetName}
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
        [HttpPost("importRootDomain/{targetName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ImportRootDomain(string targetName, IFormFile file, CancellationToken cancellationToken)
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

            await this.targetService.UploadRootDomainAsync(target, uploadRootDomain, cancellationToken);

            return Ok(uploadRootDomain.Name);
        }
    }
}
