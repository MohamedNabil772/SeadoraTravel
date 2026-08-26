namespace Seadora.Contracts.Messaging;

public interface IIntegrationEvent
{
    Guid Id { get; }
    DateTime OccurredUtc { get; }
}
