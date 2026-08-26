using Microsoft.EntityFrameworkCore;

namespace Seadora.Common.Messaging.Idempotency;

public sealed class IdempotentConsumer(IProcessedMessageDbContext ctx) : IIdempotentConsumer
{
    public Task<bool> AlreadyProcessed(Guid messageId, string consumerName, CancellationToken ct = default) =>
        ctx.ProcessedMessages.AnyAsync(p => p.MessageId == messageId && p.ConsumerName == consumerName, ct);

    public Task MarkProcessed(Guid messageId, string consumerName, CancellationToken ct = default)
    {
        ctx.ProcessedMessages.Add(new ProcessedMessage
        {
            MessageId = messageId,
            ConsumerName = consumerName,
            ProcessedUtc = DateTime.UtcNow
        });
        // ponytail: concurrent duplicates surface as a composite-PK violation; add catch/ignore only if it happens
        return ((DbContext)ctx).SaveChangesAsync(ct);
    }
}
