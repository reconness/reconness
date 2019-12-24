using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReconNess.Core.Services;
using ReconNess.Entities;
using ReconNess.Web.Dtos;

namespace ReconNess.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ICategoryService categoryService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="categoryService"></param>
        public CategoriesController(
            IMapper mapper,
            ICategoryService categoryService)
        {
            this.mapper = mapper;
            this.categoryService = categoryService;
        }

        // GET api/categories
        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var categories = await this.categoryService.GetAllByCriteriaAsync(c => !c.Deleted, cancellationToken);

            return Ok(this.mapper.Map<List<Category>, List<CategoryDto>>(categories));
        }
    }
}
