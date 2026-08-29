using MediatR;
using Microsoft.AspNetCore.Mvc;
using Seadora.Content.Application.Tours.Queries.Admin;
using Seadora.Content.Application.Tours.Commands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Seadora.Content.API.Controllers.Admin;

[ApiController]
[Route("api/admin/tours")]
public class ToursController : ControllerBase
{
    private readonly ISender _mediator;
    public ToursController(ISender mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<List<AdminTourSummaryDto>>> GetAdminTours() 
        => Ok(await _mediator.Send(new GetAdminToursQuery()));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AdminTourDetailDto>> GetAdminTourById(Guid id) 
    {
        var result = await _mediator.Send(new GetAdminTourByIdQuery(id));
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTourCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(new { id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTourCommand command)
    {
        if (id != command.Id) return BadRequest("Mismatched tour ID.");
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteTourCommand(id));
        return NoContent();
    }

    [HttpGet("favorites/leaderboard")]
    public async Task<IActionResult> GetFavoritesLeaderboard([FromQuery] int limit = 20)
    {
        var result = await _mediator.Send(new GetTourFavoritesLeaderboardQuery(limit));
        return Ok(result);
    }
}
