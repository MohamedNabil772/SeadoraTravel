using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    // Allow up to 100 MB for multi-megabyte Excel/PDF uploads
    options.Limits.MaxRequestBodySize = 104857600; 
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var secret = builder.Configuration["JwtSettings:Secret"] ?? builder.Configuration["Jwt:Key"] ?? "YourSuperSecretKeyHereYourSuperSecretKeyHere";
    var issuer = builder.Configuration["JwtSettings:Issuer"] ?? builder.Configuration["Jwt:Issuer"] ?? "SeadoraTravel";
    var audience = builder.Configuration["JwtSettings:Audience"] ?? builder.Configuration["Jwt:Audience"] ?? "SeadoraTravelUsers";

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret))
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole("Admin", "SuperAdmin", "BookingManager", "OperationsManager", "ConciergeSpecialist", "Accountant", "BusinessOwner");
    });
});

var app = builder.Build();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy();

app.Run();
