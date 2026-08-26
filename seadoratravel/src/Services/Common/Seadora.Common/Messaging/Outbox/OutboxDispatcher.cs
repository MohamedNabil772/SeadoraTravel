using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Seadora.Common.Messaging.Outbox;

public sealed class OutboxDispatcher<TContext>(
    IServiceScopeFactory scopeFactory,
    ILogger<OutboxDispatcher<TContext>> logger) : BackgroundService
    where TContext : DbContext, IOutboxDbContext
{
    // ponytail: fixed 5s poll, no retry/poison handling — add backoff + dead-letter when volume warrants.
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessBatchAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Outbox dispatch batch failed");
            }

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }
    }

    public async Task<int> ProcessBatchAsync(CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<TContext>();
        var publisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();

        var messages = await ctx.OutboxMessages
            .Where(m => m.ProcessedUtc == null)
            .OrderBy(m => m.OccurredUtc)
            .Take(50)
            .ToListAsync(ct);

        foreach (var msg in messages)
        {
            var type = Type.GetType(msg.Type, throwOnError: true)!;
            var evt = (IIntegrationEvent)JsonSerializer.Deserialize(msg.Payload, type)!;
            await publisher.PublishAsync(evt, ct);
            msg.ProcessedUtc = DateTime.UtcNow;
        }

        if (messages.Count > 0)
        {
            await ctx.SaveChangesAsync(ct);
        }

        return messages.Count;
    }
}
