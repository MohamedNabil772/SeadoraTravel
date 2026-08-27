using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Seadora.Common.Messaging;
using Seadora.Common.Messaging.Idempotency;
using Seadora.Common.Tenancy;
using Seadora.Customer.Application.Integration;
using Seadora.Customer.Infrastructure.Persistence;

namespace Seadora.Customer.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var dataSourceBuilder = new Npgsql.NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();

        services.AddDbContext<CustomerDbContext>(options =>
        {
            options.UseNpgsql(
                dataSource,
                b => b.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null));
            options.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        });

        services.AddScoped<Seadora.Customer.Application.Common.Interfaces.ICustomerDbContext>(
            sp => sp.GetRequiredService<CustomerDbContext>());

        services.AddHttpContextAccessor();
        services.AddSeadoraTenancy();

        services.AddSeadoraIdempotency<CustomerDbContext>();
        services.AddSeadoraMessaging(configuration, x => x.AddConsumer<BookingPlacedConsumer>());
        return services;    }
}
