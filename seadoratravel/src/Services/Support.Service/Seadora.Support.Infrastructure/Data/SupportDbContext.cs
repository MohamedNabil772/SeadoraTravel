using Microsoft.EntityFrameworkCore;
using Seadora.Common.Messaging.Idempotency;
using Seadora.Common.Messaging.Outbox;
using Seadora.Support.Application.Interfaces;
using Seadora.Support.Domain.Entities;

namespace Seadora.Support.Infrastructure.Data;

public class SupportDbContext : DbContext, ISupportDbContext
{
    public SupportDbContext(DbContextOptions<SupportDbContext> options) : base(options) { }

    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<TicketMessage> TicketMessages => Set<TicketMessage>();
    public DbSet<ProcessedMessage> ProcessedMessages => Set<ProcessedMessage>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Subject).IsRequired().HasMaxLength(255);
            entity.Property(e => e.CustomerEmail).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Channel).HasConversion<string>();
            entity.Property(e => e.Status).HasConversion<string>();
            entity.Property(e => e.Priority).HasConversion<string>();
            
            entity.HasMany(e => e.Messages)
                  .WithOne(m => m.Ticket)
                  .HasForeignKey(m => m.TicketId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TicketMessage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Body).IsRequired();
            entity.Property(e => e.MessageId).HasMaxLength(255);
        });

        modelBuilder.Entity<ProcessedMessage>().HasKey(p => new { p.MessageId, p.ConsumerName });
        modelBuilder.Entity<OutboxMessage>();
    }
}
