namespace Seadora.Common.Messaging;

public interface IIntegrationEvent
{
    Guid Id { get; }
    DateTime OccurredUtc { get; }
}
