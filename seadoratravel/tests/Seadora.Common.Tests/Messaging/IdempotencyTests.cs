using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Seadora.Common.Messaging.Idempotency;

namespace Seadora.Common.Tests.Messaging;

public sealed class TestIdempotencyContext(DbContextOptions<TestIdempotencyContext> options)
    : DbContext(options), IProcessedMessageDbContext
{
    public DbSet<ProcessedMessage> ProcessedMessages => Set<ProcessedMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplySeadoraMessaging();
}

public class IdempotencyTests
{
    private static TestIdempotencyContext NewContext() =>
        new(new DbContextOptionsBuilder<TestIdempotencyContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    [Fact]
    public async Task MarkProcessed_dedupes_per_consumer()
    {
        using var ctx = NewContext();
        IIdempotentConsumer sut = new IdempotentConsumer(ctx);
        var messageId = Guid.NewGuid();

        (await sut.AlreadyProcessed(messageId, "consumerA")).Should().BeFalse();

        await sut.MarkProcessed(messageId, "consumerA");

        (await sut.AlreadyProcessed(messageId, "consumerA")).Should().BeTrue();
        (await sut.AlreadyProcessed(messageId, "consumerB")).Should().BeFalse();
    }
}
