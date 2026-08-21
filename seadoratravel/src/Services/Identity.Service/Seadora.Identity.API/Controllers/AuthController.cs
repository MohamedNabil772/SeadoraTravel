using MediatR;
using Microsoft.AspNetCore.Mvc;
using Seadora.Identity.Application.Authentication.Commands.Login;
using Seadora.Identity.Application.Authentication.Commands.Register;
using Seadora.Identity.Application.Authentication.Commands.SendWhatsAppOtp;
using Seadora.Identity.Application.Authentication.Commands.VerifyWhatsAppOtp;

namespace Seadora.Identity.API.Controllers;

[ApiController]
[Route("api/auth")]
[Route("auth")]
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
        try
        {
            return Ok(await _mediator.Send(command));
        }
        catch (System.Exception e)
        {
            return BadRequest(new { error = e.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCommand command)
    {
        try
        {
            return Ok(await _mediator.Send(command));
        }
        catch (System.Exception e)
        {
            return BadRequest(new { error = e.Message });
        }
    }

    [HttpPost("send-otp")]
    public async Task<IActionResult> SendOtp([FromBody] SendWhatsAppOtpCommand command)
    {
        try
        {
            return Ok(await _mediator.Send(command));
        }
        catch (System.Exception e)
        {
            return BadRequest(new { error = e.Message });
        }
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyWhatsAppOtpCommand command)
    {
        try
        {
            return Ok(await _mediator.Send(command));
        }
        catch (System.Exception e)
        {
            return BadRequest(new { error = e.Message });
        }
    }

    [HttpPost("social-login")]
    public async Task<IActionResult> SocialLogin([FromBody] Seadora.Identity.Application.Authentication.Commands.SocialLogin.SocialLoginCommand command)
    {
        try
        {
            return Ok(await _mediator.Send(command));
        }
        catch (System.Exception e)
        {
            return BadRequest(new { error = e.Message });
        }
    }

    [Microsoft.AspNetCore.Authorization.Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
            ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
            ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email)?.Value;
        var name = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value
            ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Name)?.Value;
            
        var roles = User.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value).ToList();

        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        return Ok(new
        {
            Id = userId,
            Email = email,
            FullName = name,
            Roles = roles
        });
    }
}
