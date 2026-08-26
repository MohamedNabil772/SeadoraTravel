using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Seadora.Common.Messaging;

namespace Seadora.Common.Tests.Messaging;

public sealed record TestEvent(string Name) : IntegrationEvent;

public class EventPublisherTests
{
    [Fact]
    public async Task PublishAsync_publishes_event_with_runtime_type_and_metadata()
    {
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness()
            .AddScoped<IEventPublisher, MassTransitEventPublisher>()
            .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();
        await harness.Start();

        var before = DateTime.UtcNow;
        using var scope = provider.CreateScope();
        await scope.ServiceProvider.GetRequiredService<IEventPublisher>().PublishAsync(new TestEvent("hi"));

        (await harness.Published.Any<TestEvent>()).Should().BeTrue();

        var published = harness.Published.Select<TestEvent>().First().Context.Message;
        published.Name.Should().Be("hi");
        published.Id.Should().NotBeEmpty();
        published.OccurredUtc.Should().BeOnOrAfter(before).And.BeOnOrBefore(DateTime.UtcNow);
    }
}
