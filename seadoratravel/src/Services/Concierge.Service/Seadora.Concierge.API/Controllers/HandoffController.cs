using MediatR;
using Microsoft.AspNetCore.Mvc;
using Seadora.Concierge.Application.Commands;
using System;
using System.Threading.Tasks;

namespace Seadora.Concierge.API.Controllers;

[ApiController]
[Route("api/handoff")]
public class HandoffController : ControllerBase
{
    private readonly IMediator _mediator;

    public HandoffController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> HandoffToHuman([FromBody] HandoffToHumanCommand command)
    {
        var response = await _mediator.Send(command);
        return Ok(response);
    }
}
