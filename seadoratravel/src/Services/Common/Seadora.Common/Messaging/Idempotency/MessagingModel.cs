using Microsoft.EntityFrameworkCore;
using Seadora.Common.Messaging.Outbox;

namespace Seadora.Common.Messaging.Idempotency;

public static class MessagingModel
{
    public static ModelBuilder ApplySeadoraMessaging(this ModelBuilder b)
    {
        b.Entity<OutboxMessage>();
        b.Entity<ProcessedMessage>().HasKey(p => new { p.MessageId, p.ConsumerName });
        return b;
    }
}
