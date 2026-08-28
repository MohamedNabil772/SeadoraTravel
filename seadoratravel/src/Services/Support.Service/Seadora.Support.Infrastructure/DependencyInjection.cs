using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Seadora.Common.Messaging;
using Seadora.Common.Messaging.Idempotency;
using Seadora.Common.Messaging.Outbox;
using Seadora.Common.Tenancy;
using Seadora.Support.Application.Interfaces;
using Seadora.Support.Infrastructure.Data;

namespace Seadora.Support.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var dataSourceBuilder = new Npgsql.NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();

        services.AddDbContext<SupportDbContext>(options =>
        {
            options.UseNpgsql(
                dataSource,
                b => b.EnableRetryOnFailure(5, System.TimeSpan.FromSeconds(10), null));
            options.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        });
            
        services.AddScoped<ISupportDbContext>(provider => provider.GetRequiredService<SupportDbContext>());

        services.AddHttpContextAccessor();
        services.AddSeadoraTenancy();
        services.AddSeadoraIdempotency<SupportDbContext>();
        services.AddSeadoraMessaging(configuration, x =>
        {
            // Add consumers here if needed
        });
        services.AddSeadoraOutbox<SupportDbContext>();

        return services;
    }
}
