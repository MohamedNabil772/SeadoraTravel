using Seadora.Booking.Application;
using Seadora.Booking.Infrastructure;
using Seadora.Common.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Apply per-deployment brand identity (emails, WhatsApp, website, logo, feedback URL).
// Blank/missing "Branding" values fall back to the built-in Seadora defaults.
Seadora.Booking.Application.Common.Email.ContactChannels.Configure(
    builder.Configuration
        .GetSection(Seadora.Booking.Application.Common.Email.BrandingOptions.SectionName)
        .Get<Seadora.Booking.Application.Common.Email.BrandingOptions>());

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
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddHttpClient();
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
    var context = scope.ServiceProvider.GetRequiredService<Seadora.Booking.Infrastructure.Persistence.BookingDbContext>();
    await Seadora.Booking.Infrastructure.Persistence.BookingDbContextSeed.SeedAsync(context);
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
