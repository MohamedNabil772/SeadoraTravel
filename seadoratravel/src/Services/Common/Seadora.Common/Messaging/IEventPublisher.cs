namespace Seadora.Common.Messaging;

public interface IEventPublisher
{
    Task PublishAsync(IIntegrationEvent evt, CancellationToken ct = default);
}
