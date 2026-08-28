using MediatR;
using Microsoft.AspNetCore.Mvc;
using Seadora.Concierge.Application.Commands;
using System;
using System.Threading.Tasks;

namespace Seadora.Concierge.API.Controllers;

[ApiController]
[Route("api/chat")]
public class ConciergeController : ControllerBase
{
    private readonly IMediator _mediator;

    public ConciergeController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> ProcessChat([FromBody] ProcessChatCommand command)
    {
        var response = await _mediator.Send(command);
        return Ok(response);
    }
}
