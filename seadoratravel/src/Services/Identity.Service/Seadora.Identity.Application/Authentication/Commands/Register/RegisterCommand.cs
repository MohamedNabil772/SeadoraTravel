using System.Collections.Generic;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Seadora.Identity.Application.Common.Interfaces;
using Seadora.Identity.Domain.Entities;

namespace Seadora.Identity.Application.Authentication.Commands.Register;

public record RegisterCommand(string FirstName, string LastName, string Email, string Password) : IRequest<AuthResponse>;

public record AuthResponse(string Token, string Email, IList<string> Roles, string? AvatarUrl = null, string PreferredLanguage = "en");

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public RegisterCommandHandler(UserManager<User> userManager, IJwtTokenGenerator jwtTokenGenerator)
    {
        _userManager = userManager;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var user = new User { UserName = request.Email, Email = request.Email, FirstName = request.FirstName, LastName = request.LastName };
        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded) throw new Exception("Registration failed");

        await _userManager.AddToRoleAsync(user, "Customer");
        var roles = new List<string> { "Customer" };
        var token = _jwtTokenGenerator.GenerateToken(user, roles);
        return new AuthResponse(token, user.Email!, roles, user.AvatarUrl, user.PreferredLanguage);
    }
}
