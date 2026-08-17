using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Seadora.Identity.Application.Common.Interfaces;
using Seadora.Identity.Domain.Entities;

namespace Seadora.Identity.Application.Authentication.Commands.SocialLogin;

public record SocialLoginCommand(
    string Provider,
    string? IdToken,
    string Email,
    string? FullName,
    string? AvatarUrl) : IRequest<SocialAuthResponse>;

public record SocialUserDto(
    string Id,
    string Email,
    string? FullName,
    string? AvatarUrl,
    string? PhoneNumber,
    IList<string> Roles);

public record SocialAuthResponse(string Token, SocialUserDto User);

public class SocialLoginCommandHandler : IRequestHandler<SocialLoginCommand, SocialAuthResponse>
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public SocialLoginCommandHandler(UserManager<User> userManager, IJwtTokenGenerator jwtTokenGenerator)
    {
        _userManager = userManager;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<SocialAuthResponse> Handle(SocialLoginCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Email))
            throw new Exception("Email is required for social login");

        var user = await _userManager.FindByEmailAsync(request.Email);
        
        if (user == null)
        {
            user = new User 
            { 
                UserName = request.Email, 
                Email = request.Email, 
                FullName = request.FullName,
                AvatarUrl = request.AvatarUrl,
                EmailConfirmed = true,
                CreatedAt = System.DateTime.UtcNow,
                LastLoginAt = System.DateTime.UtcNow
            };
            
            // Set provider ID based on the provider
            if (request.Provider.ToLower() == "google") user.GoogleId = request.IdToken; // Simplified, normally extract from token
            else if (request.Provider.ToLower() == "facebook") user.FacebookId = request.IdToken;
            else if (request.Provider.ToLower() == "apple") user.AppleId = request.IdToken;

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded) throw new Exception("Social registration failed");
            
            await _userManager.AddToRoleAsync(user, "Customer");
        }
        else
        {
            // Update last login
            user.LastLoginAt = System.DateTime.UtcNow;
            if (string.IsNullOrEmpty(user.AvatarUrl) && !string.IsNullOrEmpty(request.AvatarUrl))
                user.AvatarUrl = request.AvatarUrl;
            
            await _userManager.UpdateAsync(user);
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtTokenGenerator.GenerateToken(user, roles);

        var userDto = new SocialUserDto(
            user.Id,
            user.Email!,
            user.FullName,
            user.AvatarUrl,
            user.PhoneNumber,
            roles);

        return new SocialAuthResponse(token, userDto);
    }
}
