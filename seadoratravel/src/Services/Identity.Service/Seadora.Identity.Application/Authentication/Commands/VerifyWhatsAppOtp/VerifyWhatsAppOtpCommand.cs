using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Seadora.Identity.Application.Common.Interfaces;
using Seadora.Identity.Domain.Entities;

namespace Seadora.Identity.Application.Authentication.Commands.VerifyWhatsAppOtp;

public record VerifyWhatsAppOtpCommand(string PhoneNumber, string OtpCode) : IRequest<VerifyWhatsAppOtpResponse>;

public record UserDto(string Id, string Name, string Phone, string Email, IList<string> Roles);

public record VerifyWhatsAppOtpResponse(string Token, UserDto User, string RefreshToken);

public class VerifyWhatsAppOtpCommandHandler : IRequestHandler<VerifyWhatsAppOtpCommand, VerifyWhatsAppOtpResponse>
{
    private readonly IMemoryCache _cache;
    private readonly UserManager<User> _userManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public VerifyWhatsAppOtpCommandHandler(
        IMemoryCache cache, 
        UserManager<User> userManager, 
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _cache = cache;
        _userManager = userManager;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<VerifyWhatsAppOtpResponse> Handle(VerifyWhatsAppOtpCommand request, CancellationToken cancellationToken)
    {
        var cacheKey = $"OTP_{request.PhoneNumber}";
        if (!_cache.TryGetValue(cacheKey, out string? cachedOtp) || cachedOtp != request.OtpCode)
        {
            throw new Exception("Invalid or expired OTP");
        }

        // OTP is valid, remove from cache
        _cache.Remove(cacheKey);

        var user = _userManager.Users.FirstOrDefault(u => u.PhoneNumber == request.PhoneNumber);
        
        if (user == null)
        {
            user = new User
            {
                UserName = request.PhoneNumber,
                PhoneNumber = request.PhoneNumber,
                PhoneNumberConfirmed = true,
                FirstName = "New",
                LastName = "User"
            };

            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                throw new Exception("Failed to create user");
            }
            
            await _userManager.AddToRoleAsync(user, "Customer");
        }
        else if (!user.PhoneNumberConfirmed)
        {
            user.PhoneNumberConfirmed = true;
            await _userManager.UpdateAsync(user);
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtTokenGenerator.GenerateToken(user, roles);

        // Dummy refresh token for now, or generate properly if logic exists
        var refreshToken = Guid.NewGuid().ToString("N");

        var userDto = new UserDto(
            user.Id, 
            user.FullName ?? $"{user.FirstName} {user.LastName}".Trim(), 
            user.PhoneNumber ?? "", 
            user.Email ?? "", 
            roles);

        return new VerifyWhatsAppOtpResponse(token, userDto, refreshToken);
    }
}
