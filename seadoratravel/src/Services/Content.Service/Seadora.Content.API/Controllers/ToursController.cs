using MediatR;
using Microsoft.AspNetCore.Mvc;
using Seadora.Content.Application.Tours.Queries.GetTours;
using Seadora.Content.Application.Tours.Queries.GetTourById;
using Seadora.Content.Application.Tours.Commands;
using Seadora.Content.Domain.Entities;

namespace Seadora.Content.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ToursController : ControllerBase
{
    private readonly ISender _mediator;
    public ToursController(ISender mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<List<Tour>>> Get() => Ok(await _mediator.Send(new GetToursQuery()));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Tour>> GetById(Guid id) => Ok(await _mediator.Send(new GetTourByIdQuery(id)));

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
