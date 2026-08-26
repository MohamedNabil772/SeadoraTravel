namespace Seadora.Common.Messaging.Idempotency;

public class ProcessedMessage
{
    public Guid MessageId { get; set; }
    public string ConsumerName { get; set; } = default!;
    public DateTime ProcessedUtc { get; set; }
}
