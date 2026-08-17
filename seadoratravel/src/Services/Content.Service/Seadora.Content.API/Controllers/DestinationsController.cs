using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seadora.Content.Application.Destinations.Commands;
using Seadora.Content.Application.Destinations.Queries;
using Seadora.Content.Application.DTOs;

namespace Seadora.Content.API.Controllers;

[ApiController]
[Route("api/admin/destinations")]
[Authorize(Policy = "AdminOnly")]
public class DestinationsController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<DestinationDto>>> Get() => Ok(await mediator.Send(new GetDestinationsQuery()));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DestinationDto>> GetById(Guid id) => Ok(await mediator.Send(new GetDestinationByIdQuery(id)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDestinationCommand command) => Ok(new { id = await mediator.Send(command) });

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDestinationCommand command)
    {
        if (id != command.Id) return BadRequest("Mismatched destination ID.");
        await mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await mediator.Send(new DeleteDestinationCommand(id));
        return NoContent();
    }
}
