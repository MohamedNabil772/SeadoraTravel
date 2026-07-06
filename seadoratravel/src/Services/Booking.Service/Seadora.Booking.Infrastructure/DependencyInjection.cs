using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Seadora.Booking.Application.Common.Interfaces;
using Seadora.Booking.Infrastructure.Persistence;

namespace Seadora.Booking.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BookingDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)));
            
        services.AddScoped<IBookingDbContext>(provider => provider.GetRequiredService<BookingDbContext>());
        
        return services;
    }
}
