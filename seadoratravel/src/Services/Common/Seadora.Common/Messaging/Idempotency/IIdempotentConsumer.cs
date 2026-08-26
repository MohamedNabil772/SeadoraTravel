namespace Seadora.Common.Messaging.Idempotency;

public interface IIdempotentConsumer
{
    Task<bool> AlreadyProcessed(Guid messageId, string consumerName, CancellationToken ct = default);
    Task MarkProcessed(Guid messageId, string consumerName, CancellationToken ct = default);
}
