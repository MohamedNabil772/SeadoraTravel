using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Seadora.Support.Domain.Entities;
using Seadora.Common.Messaging.Idempotency;
using Seadora.Common.Messaging.Outbox;

namespace Seadora.Support.Application.Interfaces;

public interface ISupportDbContext : IProcessedMessageDbContext, IOutboxDbContext
{
    DbSet<Ticket> Tickets { get; }
    DbSet<TicketMessage> TicketMessages { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
