using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReconNess.Application.Services;
using ReconNess.Web.Dtos;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Web.Controllers;

[Authorize]
[Produces("application/json")]
[Route("api/[controller]")]
[ApiController]
public class SearchController : ControllerBase
{
    private readonly ISubdomainService subdomainService;
    private readonly IRootDomainService rootDomainService;
    private readonly ITargetService targetService;
    private readonly IAgentService agentService;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventTrackController" /> class
    /// </summary>
    /// <param name="subdomainService"><see cref="ISubdomainService"/></param>
    /// <param name="rootDomainService"><see cref="IRootDomainService"/></param>
    /// <param name="targetService"><see cref="ITargetService"/></param>
    /// <param name="agentService"><see cref="IAgentService"/></param>
    public SearchController(
        ISubdomainService subdomainService, 
        IRootDomainService rootDomainService,
        ITargetService targetService,
        IAgentService agentService)
    {
        this.subdomainService = subdomainService;  
        this.rootDomainService = rootDomainService;
        this.targetService = targetService;
        this.agentService = agentService;
    }

    /// <summary>
    /// Obtain the list of subdomains using {words} as a filter.
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET api/Search/subdomain/{words}
    ///
    /// </remarks>
    /// <param name="words">The search words</param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>The list of subdomains using {words} as a filter.</returns>
    /// <response code="200">Returns the list of subdomains using {words} as a filter.</response>
    /// <response code="401">If the user is not authenticate</response>
    [HttpGet("subdomain/{words}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SearchInSubdomain([FromRoute] string words, CancellationToken cancellationToken)
    {
        var result = await this.subdomainService.GetAllQueryableByCriteria(s => s.Name.Contains(words))
            .Include(s => s.RootDomain)
            .ThenInclude(s => s.Target)
            .Select(s => new SearchDto
            {
                Name = s.Name,
                RelativeUrl = $"{s.RootDomain.Target.Name}/{s.RootDomain.Name}/{s.Name}"
            })
            .ToListAsync(cancellationToken);

        return Ok(result);
    }


    /// <summary>
    /// Obtain the list of rootdomain using {words} as a filter.
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET api/Search/rootdomain/{words}
    ///
    /// </remarks>
    /// <param name="words">The search words</param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>The list of rootdomain using {words} as a filter.</returns>
    /// <response code="200">Returns the list of rootdomain using {words} as a filter.</response>
    /// <response code="401">If the user is not authenticate</response>
    [HttpGet("rootdomain/{words}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SearchInRootdomain([FromRoute] string words, CancellationToken cancellationToken)
    {
        var result = await this.rootDomainService.GetAllQueryableByCriteria(s => s.Name.Contains(words))
            .Include(s => s.Target)
            .Select(s => new SearchDto
            {
                Name = s.Name,
                RelativeUrl = $"{s.Target.Name}/{s.Name}"
            })
            .ToListAsync(cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Obtain the list of target using {words} as a filter.
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET api/Search/target/{words}
    ///
    /// </remarks>
    /// <param name="words">The search words</param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>The list of target using {words} as a filter</returns>
    /// <response code="200">Returns the list of target using {words} as a filter</response>
    /// <response code="401">If the user is not authenticate</response>
    [HttpGet("target/{words}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SearchInTarget([FromRoute] string words, CancellationToken cancellationToken)
    {
        var result = await this.targetService.GetAllQueryableByCriteria(s => s.Name.Contains(words))
            .Select(s => new SearchDto
            {
                Name = s.Name,
                RelativeUrl = $"{s.Name}"
            })
            .ToListAsync(cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Obtain the list of agent using {words} as a filter.
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET api/Search/agent/{words}
    ///
    /// </remarks>
    /// <param name="words">The search words</param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>The list of agent using {words} as a filter.</returns>
    /// <response code="200">Returns the list of agent using {words} as a filter.</response>
    /// <response code="401">If the user is not authenticate</response>
    [HttpGet("agents/{words}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SearchInAgent([FromRoute] string words, CancellationToken cancellationToken)
    {
        var result = await this.agentService.GetAllQueryableByCriteria(s => s.Name.Contains(words))
            .Select(s => new SearchDto
            {
                Name = s.Name,
                RelativeUrl = $"agents/{s.Name}"
            })
            .ToListAsync(cancellationToken);

        return Ok(result);
    }
}
