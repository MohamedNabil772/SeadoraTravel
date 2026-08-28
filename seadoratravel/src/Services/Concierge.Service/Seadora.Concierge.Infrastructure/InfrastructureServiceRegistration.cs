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
        services.AddDbContext<ConciergeDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("ConciergeDb") ?? "Host=localhost;Database=ConciergeDb;Username=postgres;Password=postgres"));

        services.AddScoped<IConciergeDbContext>(provider => provider.GetRequiredService<ConciergeDbContext>());

        return services;
    }
}
