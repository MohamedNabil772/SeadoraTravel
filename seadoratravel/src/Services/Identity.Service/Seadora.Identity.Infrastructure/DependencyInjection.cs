using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Seadora.Identity.Application.Common.Interfaces;
using Seadora.Identity.Domain.Entities;
using Seadora.Identity.Infrastructure.Authentication;
using Seadora.Identity.Infrastructure.Persistence;

namespace Seadora.Identity.Infrastructure;

public class SeadoraIdentityDbContext : Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext<User, Role, string>
{
    public SeadoraIdentityDbContext(DbContextOptions<SeadoraIdentityDbContext> options) : base(options) { }
}

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<SeadoraIdentityDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null));
            options.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        });

        services.AddIdentity<User, Role>()
            .AddEntityFrameworkStores<SeadoraIdentityDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        var secret = configuration["JwtSettings:Secret"] ?? "YourSuperSecretKeyHereYourSuperSecretKeyHere";
        var key = Encoding.ASCII.GetBytes(secret);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["JwtSettings:Issuer"] ?? "SeadoraTravel",
                ValidAudience = configuration["JwtSettings:Audience"] ?? "SeadoraTravelUsers",
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
        });

        return services;
    }
}
