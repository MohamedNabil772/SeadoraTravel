using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Seadora.Common.Messaging;
using Seadora.Common.Messaging.Outbox;
using Seadora.Contracts.Messaging;

namespace Seadora.Common.Tests.Messaging;

public sealed record OutboxTestEvent(string Name) : IntegrationEvent;

public sealed class TestOutboxContext(DbContextOptions<TestOutboxContext> options)
    : DbContext(options), IOutboxDbContext
{
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
}

public class OutboxDispatcherTests
{
    private static ServiceProvider BuildProvider(Mock<IEventPublisher> publisher)
    {
        var dbName = Guid.NewGuid().ToString();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDbContext<TestOutboxContext>(o => o.UseInMemoryDatabase(dbName));
        services.AddScoped<IOutboxDbContext>(sp => sp.GetRequiredService<TestOutboxContext>());
        services.AddScoped<IOutboxWriter, OutboxWriter>();
        services.AddSingleton(publisher.Object);
        services.AddSingleton<OutboxDispatcher<TestOutboxContext>>();
        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task ProcessBatchAsync_publishes_pending_events_exactly_once()
    {
        var publisher = new Mock<IEventPublisher>();
        await using var provider = BuildProvider(publisher);

        using (var scope = provider.CreateScope())
        {
            scope.ServiceProvider.GetRequiredService<IOutboxWriter>().Enqueue(new OutboxTestEvent("hi"));
            await scope.ServiceProvider.GetRequiredService<TestOutboxContext>().SaveChangesAsync();
        }

        var dispatcher = provider.GetRequiredService<OutboxDispatcher<TestOutboxContext>>();

        (await dispatcher.ProcessBatchAsync(default)).Should().Be(1);
        publisher.Verify(p => p.PublishAsync(
            It.Is<IIntegrationEvent>(e => ((OutboxTestEvent)e).Name == "hi"),
            It.IsAny<CancellationToken>()), Times.Once);

        using (var scope = provider.CreateScope())
        {
            var row = await scope.ServiceProvider.GetRequiredService<TestOutboxContext>()
                .OutboxMessages.SingleAsync();
            row.ProcessedUtc.Should().NotBeNull();
        }

        (await dispatcher.ProcessBatchAsync(default)).Should().Be(0);
        publisher.Verify(p => p.PublishAsync(It.IsAny<IIntegrationEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
