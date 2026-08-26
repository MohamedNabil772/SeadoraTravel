using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Seadora.Content.Application;
using Seadora.Content.Infrastructure;
using Seadora.Common.Middlewares;
var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    // Allow up to 100 MB for Excel imports downstream
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

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"] ?? "SeadoraTravel",
            ValidAudience = builder.Configuration["JwtSettings:Audience"] ?? "SeadoraTravelUsers",
            // TODO security: the signing secret must come from a secret store / env var in production.
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
                builder.Configuration["JwtSettings:Secret"] ?? "YourSuperSecretKeyHereYourSuperSecretKeyHere"))
        };
    });

var adminPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
    .RequireAuthenticatedUser()
    .RequireRole("Admin", "SuperAdmin")
    .Build();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", adminPolicy);
    // ponytail: fail closed - everything requires AdminPolicy unless it opts out with [AllowAnonymous].
    options.FallbackPolicy = adminPolicy;
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<Seadora.Content.Infrastructure.Persistence.ContentDbContext>();
    await Seadora.Content.Infrastructure.Persistence.ContentSeeder.InitializeAsync(context);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSeadoraExceptionHandler();

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
