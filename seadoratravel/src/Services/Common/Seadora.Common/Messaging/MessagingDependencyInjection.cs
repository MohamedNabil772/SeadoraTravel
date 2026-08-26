using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Seadora.Common.Messaging;

public static class MessagingDependencyInjection
{
    public static IServiceCollection AddSeadoraMessaging(this IServiceCollection services, IConfiguration config)
    {
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(config["RabbitMq:Host"] ?? "localhost", h =>
                {
                    h.Username(config["RabbitMq:Username"] ?? "seadora");
                    h.Password(config["RabbitMq:Password"] ?? "seadora");
                });
                cfg.ConfigureEndpoints(context);
            });
        });

        services.AddScoped<IEventPublisher, MassTransitEventPublisher>();
        return services;
    }
}
