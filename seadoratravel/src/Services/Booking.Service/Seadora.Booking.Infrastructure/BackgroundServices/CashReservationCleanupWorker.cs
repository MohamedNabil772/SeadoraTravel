using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Seadora.Booking.Application.Common.Interfaces;
using Seadora.Booking.Domain.Enums;
using Seadora.Booking.Domain.Services;

namespace Seadora.Booking.Infrastructure.BackgroundServices;

public class CashReservationCleanupWorker : BackgroundService
{
    private readonly ILogger<CashReservationCleanupWorker> _logger;
    private readonly IServiceProvider _serviceProvider;

    public CashReservationCleanupWorker(ILogger<CashReservationCleanupWorker> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("CashReservationCleanupWorker starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("CashReservationCleanupWorker running at: {time}", DateTimeOffset.Now);

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<IBookingDbContext>();
                var policyService = scope.ServiceProvider.GetRequiredService<ICancellationPolicyService>();

                var pendingCashBookings = await context.Bookings
                    .Where(b => b.Status == BookingStatus.Pending && !b.IsPaid)
                    .ToListAsync(stoppingToken);

                int cancelledCount = 0;
                var now = DateTime.UtcNow;

                foreach (var booking in pendingCashBookings)
                {
                    if (!policyService.IsCashReservationValid(booking, now))
                    {
                        booking.Status = BookingStatus.Cancelled;
                        _logger.LogInformation("Auto-cancelling cash booking {BookingId} as it is within 48 hours of the tour.", booking.Id);
                        cancelledCount++;
                    }
                }

                if (cancelledCount > 0)
                {
                    await context.SaveChangesAsync(stoppingToken);
                    _logger.LogInformation("Successfully cancelled {Count} cash bookings.", cancelledCount);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred executing CashReservationCleanupWorker.");
            }

            // Run every hour
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}
