using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seadora.Content.Application.Tours.Queries.GetTours;
using Seadora.Content.Application.Tours.Queries.GetTourById;
using Seadora.Content.Application.Tours.Commands;
using Seadora.Content.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Seadora.Content.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ToursController : ControllerBase
{
    private readonly ISender _mediator;
    public ToursController(ISender mediator) => _mediator = mediator;

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<TourSummaryDto>>> Get(
        [FromQuery] string? search, 
        [FromQuery] DateTime? startDate, 
        [FromQuery] DateTime? endDate,
        [FromQuery] string? destination,
        [FromQuery] string? category,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] string language = "en"
    ) => 
        Ok(await _mediator.Send(new GetToursQuery(search, startDate, endDate, destination, category, minPrice, maxPrice, language)));

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<TourDto>> GetById(Guid id) 
    {
        var result = await _mediator.Send(new GetTourByIdQuery(id));
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
}
