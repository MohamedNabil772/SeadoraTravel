using Microsoft.EntityFrameworkCore;

namespace Seadora.Common.Messaging.Outbox;

public interface IOutboxDbContext
{
    DbSet<OutboxMessage> OutboxMessages { get; }
}
