using Microsoft.EntityFrameworkCore;

namespace Seadora.Common.Messaging.Idempotency;

public interface IProcessedMessageDbContext
{
    DbSet<ProcessedMessage> ProcessedMessages { get; }
}
