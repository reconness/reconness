using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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

        // GET api/references
        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var references = await this.referenceService.GetReferencesAsync(cancellationToken);

            var agentsDto = this.mapper.Map<List<Reference>, List<ReferenceDto>>(references);

            return Ok(agentsDto);
        }

        // GET api/references/categories
        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
        {
            var categories = await this.referenceService.GetAllCategoriesAsync(cancellationToken);

            return Ok(categories);
        }

        // POST api/references
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ReferenceDto referenceDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var reference = this.mapper.Map<ReferenceDto, Reference>(referenceDto);

            await this.referenceService.AddAsync(reference, cancellationToken);

            return NoContent();
        }

        // DELETE api/references/{id}
        [HttpDelete("{id}")]
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
