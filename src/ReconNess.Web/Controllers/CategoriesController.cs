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
    public class CategoriesController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IAgentCategoryService categoryService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoriesController" /> class
        /// </summary>
        /// <param name="mapper"><see cref="IMapper"/></param>
        /// <param name="categoryService"><see cref="IAgentCategoryService"/></param>
        public CategoriesController(
            IMapper mapper,
            IAgentCategoryService categoryService)
        {
            this.mapper = mapper;
            this.categoryService = categoryService;
        }

        // GET api/categories
        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var categories = await this.categoryService
                        .GetAllQueryable(cancellationToken)
                        .AsNoTracking()
                        .ToListAsync(cancellationToken);

            return Ok(this.mapper.Map<List<Category>, List<CategoryDto>>(categories));
        }
    }
}
