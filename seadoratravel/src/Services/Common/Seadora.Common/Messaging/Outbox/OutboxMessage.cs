namespace Seadora.Common.Messaging.Outbox;

public class OutboxMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Type { get; set; } = default!;
    public string Payload { get; set; } = default!;
    public DateTime OccurredUtc { get; set; }
    public DateTime? ProcessedUtc { get; set; }
}
