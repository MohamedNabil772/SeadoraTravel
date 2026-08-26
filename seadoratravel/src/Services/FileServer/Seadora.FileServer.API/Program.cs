using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Seadora.Common.Storage;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();
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

// Configure Storage
var storagePath = builder.Configuration["StorageSettings:Path"] ?? "uploads";
builder.Services.AddSingleton<IStorageService>(new LocalStorageService(storagePath));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
