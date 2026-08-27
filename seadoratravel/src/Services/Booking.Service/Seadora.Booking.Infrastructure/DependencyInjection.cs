using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Seadora.Booking.Application.Common.Interfaces;
using Seadora.Booking.Application.Integration;
using Seadora.Booking.Infrastructure.Configuration;
using Seadora.Booking.Infrastructure.Persistence;
using Seadora.Booking.Infrastructure.Services;
using Seadora.Common.Messaging;
using Seadora.Common.Messaging.Idempotency;
using Seadora.Common.Messaging.Outbox;
using Seadora.Common.Tenancy;

namespace Seadora.Booking.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var dataSourceBuilder = new Npgsql.NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();

        services.AddDbContext<BookingDbContext>(options =>
        {
            options.UseNpgsql(
                dataSource,
                b => b.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null));
            options.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        });
            
        services.AddScoped<IBookingDbContext>(provider => provider.GetRequiredService<BookingDbContext>());

        services.AddHttpContextAccessor();
        services.AddSeadoraTenancy();
        services.AddSeadoraIdempotency<BookingDbContext>();
        services.AddSeadoraMessaging(configuration, x =>
        {
            x.AddConsumer<TourProjectionConsumers>();
            x.AddConsumer<PaymentRecordedConsumer>();
        });
        services.AddSeadoraOutbox<BookingDbContext>();

        services.AddHostedService<Seadora.Booking.Infrastructure.BackgroundServices.CashReservationCleanupWorker>();
        
        services.Configure<TwilioSettings>(configuration.GetSection(TwilioSettings.SectionName));
        services.AddHttpClient<IWhatsAppNotificationService, TwilioWhatsAppService>();
        
        services.Configure<SmtpSettings>(configuration.GetSection(SmtpSettings.SectionName));
        services.AddScoped<IEmailSender, Seadora.Booking.Infrastructure.Email.SmtpEmailSender>();
        
        return services;
    }
}
