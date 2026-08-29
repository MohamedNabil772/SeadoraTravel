using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Seadora.Content.Application.Currencies;

namespace Seadora.Content.Infrastructure.BackgroundServices;

public class DailyExchangeRateSyncWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DailyExchangeRateSyncWorker> _logger;
    private static readonly TimeSpan SyncInterval = TimeSpan.FromMinutes(30);

    public DailyExchangeRateSyncWorker(
        IServiceProvider serviceProvider,
        ILogger<DailyExchangeRateSyncWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ExchangeRateSyncWorker initialized. Scheduled every {Minutes} minutes.", SyncInterval.TotalMinutes);

        // Initial sync on startup after 10s delay to let DB seed and settle
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            await RunSyncAsync(stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            return;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Initial exchange rate sync failed.");
        }

        using var timer = new PeriodicTimer(SyncInterval);

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await RunSyncAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Daily exchange rate sync failed.");
            }
        }
    }

    private async Task RunSyncAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Executing automated daily exchange rate synchronization...");
        using var scope = _serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var result = await mediator.Send(new SyncLiveExchangeRatesCommand(), cancellationToken);
        _logger.LogInformation("Successfully synced exchange rates for {Count} active currencies (manual overrides preserved).", result.Count);
    }
}
