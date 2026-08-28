using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Seadora.Concierge.Application.Commands;
using Seadora.Concierge.Infrastructure.Data;

namespace Seadora.Concierge.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddConciergeInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connStr = configuration.GetConnectionString("ConciergeDb")
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Database=ConciergeDb;Username=postgres;Password=postgres";

        services.AddDbContext<ConciergeDbContext>(options =>
            options.UseNpgsql(connStr));

        services.AddScoped<IConciergeDbContext>(provider => provider.GetRequiredService<ConciergeDbContext>());

        return services;
    }
}
