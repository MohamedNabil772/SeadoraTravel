using MassTransit;

namespace Seadora.Common.Messaging;

public sealed class MassTransitEventPublisher(IPublishEndpoint endpoint) : IEventPublisher
{
    // ponytail: publish by runtime type so derived event types route to their own exchange
    public Task PublishAsync(IIntegrationEvent evt, CancellationToken ct = default)
        => endpoint.Publish(evt, evt.GetType(), ct);
}
