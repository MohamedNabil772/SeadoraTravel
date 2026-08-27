using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Seadora.Identity.Application.Common.Interfaces;
using Seadora.Common.Tenancy;
using Seadora.Identity.Domain.Entities;

namespace Seadora.Identity.Infrastructure.Authentication;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration;
    private readonly SeadoraIdentityDbContext _context;

    public JwtTokenGenerator(IConfiguration configuration, SeadoraIdentityDbContext context)
    {
        _configuration = configuration;
        _context = context;
    }

    public string GenerateToken(User user, IList<string> roles)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var secret = _configuration["JwtSettings:Secret"] ?? "YourSuperSecretKeyHereYourSuperSecretKeyHere";
        var key = Encoding.ASCII.GetBytes(secret);
        
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(JwtRegisteredClaimNames.Name, !string.IsNullOrEmpty(user.FullName) ? user.FullName : $"user.FirstName user.LastName".Trim()),
            new("firstName", user.FirstName),
            new("lastName", user.LastName),
            new(SeadoraBranches.BranchClaimType, SeadoraBranches.HeadOfficeClaimValue)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
            if (role == "SuperAdmin")
            {
                claims.Add(new Claim("permission", "*"));
                // ponytail: SuperAdmin is the top role; grant the legacy "Admin" role claim so
                // every existing [Authorize(Roles="Admin")] gate (e.g. UsersController) accepts it.
                if (!roles.Contains("Admin"))
                {
                    claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                }
            }
        }

        if (!roles.Contains("SuperAdmin"))
        {
            var roleIds = _context.Roles.Where(r => roles.Contains(r.Name!)).Select(r => r.Id).ToList();
            var permissions = _context.RolePermissions
                .Where(rp => roleIds.Contains(rp.RoleId))
                .Select(rp => rp.PermissionId)
                .Distinct()
                .ToList();

            foreach (var perm in permissions)
            {
                claims.Add(new Claim("permission", perm));
            }
        }

        var expiryMinutes = 60;

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _configuration["JwtSettings:Issuer"] ?? "SeadoraTravel",
            Audience = _configuration["JwtSettings:Audience"] ?? "SeadoraTravelUsers"
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
