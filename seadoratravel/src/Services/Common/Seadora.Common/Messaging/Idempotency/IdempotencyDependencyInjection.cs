using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Seadora.Common.Messaging.Idempotency;

public static class IdempotencyDependencyInjection
{
    public static IServiceCollection AddSeadoraIdempotency<TContext>(this IServiceCollection services)
        where TContext : DbContext, IProcessedMessageDbContext
    {
        services.AddScoped<IProcessedMessageDbContext>(sp => sp.GetRequiredService<TContext>());
        services.AddScoped<IIdempotentConsumer, IdempotentConsumer>();
        return services;
    }
}
