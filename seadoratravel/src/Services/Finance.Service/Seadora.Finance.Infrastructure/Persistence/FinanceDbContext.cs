using Microsoft.EntityFrameworkCore;
using Seadora.Common.Messaging.Idempotency;

namespace Seadora.Finance.Infrastructure.Persistence;

// ponytail: skeleton context - no domain DbSets yet, only the idempotency table Common needs.
public class FinanceDbContext : DbContext, IProcessedMessageDbContext
{
    public FinanceDbContext(DbContextOptions<FinanceDbContext> options) : base(options) { }

    public DbSet<ProcessedMessage> ProcessedMessages => Set<ProcessedMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FinanceDbContext).Assembly);

        modelBuilder.Entity<ProcessedMessage>().HasKey(p => new { p.MessageId, p.ConsumerName });
    }
}
