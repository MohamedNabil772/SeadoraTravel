using MediatR;
using Microsoft.AspNetCore.Mvc;
using Seadora.Content.Application.Tours.Queries.GetTours;
using Seadora.Content.Application.Tours.Queries.GetTourById;
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
    public async Task<ActionResult<TourDto>> GetById(Guid id) 
    {
        var result = await _mediator.Send(new GetTourByIdQuery(id));
        if (result == null) return NotFound();
        return Ok(result);
    }
}
