using MediatR;
using Microsoft.AspNetCore.Mvc;
using Seadora.Content.Application.Destinations.Queries.GetDestinations;
using Seadora.Content.Application.Destinations.Queries.GetDestinationById;
using Seadora.Content.Application.Destinations.Commands;
using Seadora.Content.Domain.Entities;

namespace Seadora.Content.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DestinationsController : ControllerBase
{
    private readonly ISender _mediator;
    public DestinationsController(ISender mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<List<Destination>>> Get() => Ok(await _mediator.Send(new GetDestinationsQuery()));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Destination>> GetById(Guid id) => Ok(await _mediator.Send(new GetDestinationByIdQuery(id)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDestinationCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(new { id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDestinationCommand command)
    {
        if (id != command.Id) return BadRequest("Mismatched destination ID.");
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteDestinationCommand(id));
        return NoContent();
    }
}
