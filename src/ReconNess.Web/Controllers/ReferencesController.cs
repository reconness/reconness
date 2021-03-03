using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReconNess.Core.Services;
using ReconNess.Entities;
using ReconNess.Web.Dtos;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Web.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class ReferencesController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IReferenceService referenceService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferencesController" /> class
        /// </summary>
        /// <param name="mapper"><see cref="IMapper"/></param>
        /// <param name="referenceService"><see cref="IReferenceService"/></param>
        public ReferencesController(
            IMapper mapper,
            IReferenceService referenceService)
        {
            this.mapper = mapper;
            this.referenceService = referenceService;
        }

        /// <summary>
        /// Obtain the list of references.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET api/references
        ///
        /// </remarks>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The list of references</returns>
        /// <response code="200">Returns the list of references</response>
        /// <response code="401">If the user is not authenticate</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var references = await this.referenceService.GetReferencesAsync(cancellationToken);

            var agentsDto = this.mapper.Map<List<Reference>, List<ReferenceDto>>(references);

            return Ok(agentsDto);
        }

        /// <summary>
        /// Obtain the list of reference categories.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET api/references/categories
        ///
        /// </remarks>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The list of reference categories</returns>
        /// <response code="200">Returns the list of reference categories</response>
        /// <response code="401">If the user is not authenticate</response>
        [HttpGet("categories")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
        {
            var categories = await this.referenceService.GetAllCategoriesAsync(cancellationToken);

            return Ok(categories);
        }

        /// <summary>
        /// Save a new reference.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/references
        ///     {
        ///         "url": "wwww.therefernece.com",
        ///         "categories": "category1, category2"
        ///     }
        ///
        /// </remarks>
        /// <param name="referenceDto">The reference dto</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The new reference</returns>
        /// <response code="200">Returns the new reference</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">If the user is not authenticate</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Post([FromBody] ReferenceDto referenceDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var reference = this.mapper.Map<ReferenceDto, Reference>(referenceDto);

            var referenceAdded = await this.referenceService.AddAsync(reference, cancellationToken);

            return Ok(this.mapper.Map<Reference, ReferenceDto>(referenceAdded));
        }

        /// <summary>
        /// Delete the reference.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE api/references/{id}
        ///
        /// </remarks>
        /// <param name="id">The reference id</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <response code="204">No Content</response>
        /// <response code="401">If the user is not authenticate</response>
        /// <response code="404">Not Found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]        
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var reference = await this.referenceService.GetByCriteriaAsync(t => t.Id == id, cancellationToken);
            if (reference == null)
            {
                return NotFound();
            }

            await this.referenceService.DeleteAsync(reference, cancellationToken);

            return NoContent();
        }
    }
}
