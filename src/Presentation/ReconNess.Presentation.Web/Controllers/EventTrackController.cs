using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReconNess.Application.Services;
using ReconNess.Domain.Entities;
using ReconNess.Web.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Web.Controllers;

[Authorize]
[Produces("application/json")]
[Route("api/[controller]")]
[ApiController]
public class EventTrackController : ControllerBase
{
    private readonly IMapper mapper;
    private readonly IEventTrackService eventTrackService;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventTrackController" /> class
    /// </summary>
    /// <param name="mapper"><see cref="IMapper"/></param>
    /// <param name="eventTrackService"><see cref="IEventTrackService"/></param>
    public EventTrackController(
        IMapper mapper,
        IEventTrackService eventTrackService)
    {
        this.mapper = mapper;
        this.eventTrackService = eventTrackService;
    }

    /// <summary>
    /// Obtain the list of last 7 days eventTrack.
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET api/EventTrack
    ///
    /// </remarks>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>The list of event track</returns>
    /// <response code="200">Returns the list of event track</response>
    /// <response code="401">If the user is not authenticate</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var date = DateTime.UtcNow.AddDays(-7).Date;
        var eventTracks = await eventTrackService
                    .GetAllQueryable()
                    .Where(e => e.CreatedAt >= date)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

        return Ok(mapper.Map<List<EventTrack>, List<EventTrackDto>>(eventTracks));
    }

    /// <summary>
    /// Obtain yesterdar and today unread
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET api/EventTrack/yesterdayAndToday
    ///
    /// </remarks>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>The list of event track</returns>
    /// <response code="200">Returns the list of event track</response>
    /// <response code="401">If the user is not authenticate</response>
    [HttpGet("yesterdayAndToday")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetYesterdayAndTodayUnread(CancellationToken cancellationToken)
    {
        var date = DateTime.UtcNow.AddDays(-2).Date;
        var eventTracks = await eventTrackService
                    .GetAllQueryable()
                    .Where(e => e.CreatedAt >= date && !e.Read)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

        return Ok(mapper.Map<List<EventTrack>, List<EventTrackDto>>(eventTracks));
    }

    /// <summary>
    /// Obtain the list eventTrack between before and after.
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET api/EventTrack/between/{before}/{after}
    ///
    /// </remarks>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>The list of event track</returns>
    /// <response code="200">Returns the list of event track</response>
    /// <response code="401">If the user is not authenticate</response>
    [HttpGet("between/{before}/{after}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Between([FromRoute] DateTime before, [FromRoute] DateTime after, CancellationToken cancellationToken)
    {
        var eventTracks = await eventTrackService
                    .GetAllQueryable()
                    .Where(e => e.CreatedAt > before.ToUniversalTime().Date && e.CreatedAt < after.ToUniversalTime().Date)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

        return Ok(mapper.Map<List<EventTrack>, List<EventTrackDto>>(eventTracks));
    }

    /// <summary>
    /// Mark the event track as read from yesterday and today.
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     PUT api/eventTract/markReadYesterdayAndToday
    ///
    /// </remarks>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <response code="204">Returns no content</response>
    /// <response code="401">If the user is not authenticate</response>
    [HttpPut("markReadYesterdayAndToday")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkReadYesterdayAndToday(CancellationToken cancellationToken)
    {
        var yestarday = DateTime.UtcNow.AddDays(-1).Date;
        var eventTracks = await eventTrackService.GetAllByCriteriaAsync(e => !e.Read && e.CreatedAt > yestarday, cancellationToken);
        foreach (var eventTrack in eventTracks)
        {
            eventTrack.Read = true;
        }

        await eventTrackService.UpdateRangeAsync(eventTracks, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Mark all the event track as read.
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     PUT api/eventTract/markAllRead
    ///
    /// </remarks>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <response code="204">Returns no content</response>
    /// <response code="401">If the user is not authenticate</response>
    [HttpPut("markAllRead")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAllRead(CancellationToken cancellationToken)
    {
        var eventTracks = await eventTrackService.GetAllByCriteriaAsync(e => !e.Read, cancellationToken);
        foreach (var eventTrack in eventTracks)
        {
            eventTrack.Read = true;
        }

        await eventTrackService.UpdateRangeAsync(eventTracks, cancellationToken);

        return NoContent();
    }
}
