namespace Seadora.Common.Messaging;

public abstract record IntegrationEvent : IIntegrationEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime OccurredUtc { get; init; } = DateTime.UtcNow;
}
