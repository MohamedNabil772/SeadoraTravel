using MediatR;
using Microsoft.AspNetCore.Mvc;
using Seadora.Identity.Application.Authentication.Commands.Login;
using Seadora.Identity.Application.Authentication.Commands.Register;

namespace Seadora.Identity.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ISender _mediator;

    public AuthController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterCommand command)
    {
        return Ok(await _mediator.Send(command));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCommand command)
    {
        return Ok(await _mediator.Send(command));
    }
}
