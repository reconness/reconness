using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReconNess.Application.Services;
using ReconNess.Domain.Entities;
using ReconNess.Web.Dtos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Web.Controllers;

[Authorize]
[Produces("application/json")]
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

    /// <summary>
    /// Obtain the list of agent categories.
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET api/categories
    ///
    /// </remarks>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>The list of agent categories</returns>
    /// <response code="200">Returns the list of agent categories</response>
    /// <response code="401">If the user is not authenticate</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var categories = await categoryService
                    .GetAllQueryable()
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

        return Ok(mapper.Map<List<Category>, List<CategoryDto>>(categories));
    }
}
