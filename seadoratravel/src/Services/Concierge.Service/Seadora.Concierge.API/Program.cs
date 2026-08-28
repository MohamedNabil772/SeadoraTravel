using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Seadora.Concierge.Application;
using Seadora.Concierge.Infrastructure;
using MassTransit;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddApplicationServices();
builder.Services.AddConciergeInfrastructure(builder.Configuration);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumers(typeof(Seadora.Concierge.Application.IntegrationEvents.TourCatalogConsumers).Assembly);

    x.UsingRabbitMq((context, cfg) =>
    {
        var host = builder.Configuration["RabbitMq:Host"] ?? builder.Configuration["RabbitMQ:Host"] ?? "localhost";
        var user = builder.Configuration["RabbitMq:Username"] ?? builder.Configuration["RabbitMQ:Username"] ?? "seadora";
        var pass = builder.Configuration["RabbitMq:Password"] ?? builder.Configuration["RabbitMQ:Password"] ?? "seadora";

        cfg.Host(host, "/", h =>
        {
            h.Username(user);
            h.Password(pass);
        });
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<Seadora.Concierge.Infrastructure.Data.ConciergeDbContext>();
    db.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");
app.UseRouting();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
