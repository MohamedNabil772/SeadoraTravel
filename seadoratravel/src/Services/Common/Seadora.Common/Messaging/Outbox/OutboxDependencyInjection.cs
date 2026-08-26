using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Seadora.Common.Messaging.Outbox;

public static class OutboxDependencyInjection
{
    public static IServiceCollection AddSeadoraOutbox<TContext>(this IServiceCollection services)
        where TContext : DbContext, IOutboxDbContext
    {
        services.AddScoped<IOutboxDbContext>(sp => sp.GetRequiredService<TContext>());
        services.AddScoped<IOutboxWriter, OutboxWriter>();
        services.AddHostedService<OutboxDispatcher<TContext>>();
        return services;
    }
}
