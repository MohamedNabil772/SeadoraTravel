using Microsoft.EntityFrameworkCore;
using Seadora.Common.Messaging.Idempotency;
using Seadora.Customer.Application.Common.Interfaces;
using Seadora.Customer.Domain.Entities;

namespace Seadora.Customer.Infrastructure.Persistence;

public class CustomerDbContext : DbContext, ICustomerDbContext, IProcessedMessageDbContext
{
    public CustomerDbContext(DbContextOptions<CustomerDbContext> options) : base(options) { }

    public DbSet<Seadora.Customer.Domain.Entities.Customer> Customers => Set<Seadora.Customer.Domain.Entities.Customer>();
    public DbSet<CustomerDocument> CustomerDocuments => Set<CustomerDocument>();
    public DbSet<CustomerBookingHistory> BookingHistory => Set<CustomerBookingHistory>();
    public DbSet<ProcessedMessage> ProcessedMessages => Set<ProcessedMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Seadora.Customer.Domain.Entities.Customer>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.FullName).IsRequired().HasMaxLength(200);
            entity.Property(c => c.Email).IsRequired().HasMaxLength(256);
            // ponytail: email is stored normalized, so a plain unique index is the whole
            // "one email per branch" rule - no expression index, no citext extension.
            entity.HasIndex(c => new { c.BranchId, c.Email }).IsUnique();
            entity.HasMany(c => c.Documents)
                  .WithOne()
                  .HasForeignKey(d => d.CustomerId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CustomerDocument>(entity =>
        {
            entity.HasKey(d => d.Id);
            entity.Property(d => d.DocumentType).IsRequired().HasMaxLength(50);
            entity.Property(d => d.FileRef).IsRequired().HasMaxLength(500);
            entity.Property(d => d.FileName).IsRequired().HasMaxLength(260);
        });

        // ponytail: the unique BookingId index IS the idempotent upsert key - no extra dedupe table.
        modelBuilder.Entity<CustomerBookingHistory>(entity =>
        {
            entity.HasKey(h => h.Id);
            entity.Property(h => h.Currency).IsRequired().HasMaxLength(3);
            entity.HasIndex(h => h.BookingId).IsUnique();
        });

        modelBuilder.Entity<ProcessedMessage>().HasKey(p => new { p.MessageId, p.ConsumerName });
    }
}
