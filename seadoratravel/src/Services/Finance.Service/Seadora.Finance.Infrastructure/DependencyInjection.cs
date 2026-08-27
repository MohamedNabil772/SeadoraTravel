using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Seadora.Common.Messaging;
using Seadora.Common.Messaging.Idempotency;
using Seadora.Common.Tenancy;
using Seadora.Finance.Application.Common.Interfaces;
using Seadora.Finance.Application.Integration;
using Seadora.Finance.Infrastructure.Persistence;

namespace Seadora.Finance.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var dataSourceBuilder = new Npgsql.NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();

        services.AddDbContext<FinanceDbContext>(options =>
        {
            options.UseNpgsql(
                dataSource,
                b => b.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null));
            options.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        });

        services.AddScoped<IFinanceDbContext>(sp => sp.GetRequiredService<FinanceDbContext>());

        services.AddHttpContextAccessor();
        services.AddSeadoraTenancy();

        services.AddSeadoraIdempotency<FinanceDbContext>();
        services.AddSeadoraMessaging(configuration, x =>
        {
            x.AddConsumer<FinanceEventConsumers>();
        });
        return services;
    }
}
