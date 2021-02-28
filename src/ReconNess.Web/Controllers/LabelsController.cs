using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReconNess.Core.Services;
using ReconNess.Entities;
using ReconNess.Web.Dtos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Web.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class LabelsController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ILabelService labelService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LabelsController" /> class
        /// </summary>
        /// <param name="mapper"><see cref="IMapper"/></param>
        /// <param name="labelService"><see cref="ILabelService"/></param>
        public LabelsController(
            IMapper mapper,
            ILabelService labelService)
        {
            this.mapper = mapper;
            this.labelService = labelService;
        }

        /// <summary>
        /// Obtain the notifications configuration.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET api/labels
        ///
        /// </remarks>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The notifications configuration</returns>
        /// <response code="200">Returns the notifications configuration</response>
        /// <response code="401">If the user is not authenticate</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var labels = await this.labelService.GetAllQueryable(cancellationToken)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return Ok(this.mapper.Map<List<Label>, List<LabelDto>>(labels));
        }
    }
}
