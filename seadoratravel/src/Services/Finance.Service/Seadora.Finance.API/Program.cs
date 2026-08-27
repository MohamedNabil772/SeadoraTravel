using Seadora.Finance.Application;
using Seadora.Finance.Infrastructure;
using Seadora.Common.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

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

// ponytail: one loop over the ids instead of six copy-pasted policy blocks.
string[] financePermissions =
[
    "Finance.ViewDashboard",
    "Finance.ViewReports",
    "Finance.ManagePayments",
    "Finance.PostAdjustments",
    "Finance.Reconcile",
    "Finance.Export"
];

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", adminPolicy);

    foreach (var permission in financePermissions)
    {
        options.AddPolicy(permission, p => p.RequireAuthenticatedUser().RequireAssertion(ctx =>
            ctx.User.HasClaim(c => c.Type == "permission" && (c.Value == permission || c.Value == "*"))));
    }

    // ponytail: fail closed - everything requires AdminPolicy unless it opts out with [AllowAnonymous].
    options.FallbackPolicy = adminPolicy;
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<Seadora.Finance.Infrastructure.Persistence.FinanceDbContext>();
    db.Database.Migrate();
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

app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "finance" })).AllowAnonymous();

app.Run();
