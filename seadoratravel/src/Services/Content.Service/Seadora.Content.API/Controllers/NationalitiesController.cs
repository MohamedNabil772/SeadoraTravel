using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seadora.Content.Application.Nationalities;

namespace Seadora.Content.API.Controllers;

[ApiController]
[Route("api/v1/nationalities")]
[Route("api/nationalities")]
public class NationalitiesController : ControllerBase
{
    private readonly IMediator _mediator;

    public NationalitiesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Get([FromQuery] bool includeInactive = false)
    {
        return Ok(await _mediator.Send(new GetNationalitiesQuery(includeInactive)));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateNationalityCommand command)
    {
        return Ok(await _mediator.Send(command));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateNationalityCommand command)
    {
        if (id != command.Id) return BadRequest();
        return Ok(await _mediator.Send(command));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        return Ok(await _mediator.Send(new DeleteNationalityCommand(id)));
    }

    [HttpPatch("{id}/toggle-active")]
    public async Task<IActionResult> ToggleActive(Guid id, ToggleNationalityActiveCommand command)
    {
        if (id != command.Id) return BadRequest();
        return Ok(await _mediator.Send(command));
    }
}
