using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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

        // GET api/labels
        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var labels = await this.labelService.GetAllQueryable(cancellationToken)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return Ok(this.mapper.Map<List<Label>, List<LabelDto>>(labels));
        }
    }
}
