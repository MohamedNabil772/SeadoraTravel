using Microsoft.EntityFrameworkCore;
using Seadora.Booking.Application.Common.Interfaces;
using Seadora.Booking.Domain.Entities;
using Seadora.Common.Messaging.Idempotency;
using Seadora.Common.Messaging.Outbox;

namespace Seadora.Booking.Infrastructure.Persistence;

public class BookingDbContext : DbContext, IBookingDbContext, IProcessedMessageDbContext, IOutboxDbContext
{
    public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options) { }

    public DbSet<ProcessedMessage> ProcessedMessages => Set<ProcessedMessage>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<TourProjection> TourProjections => Set<TourProjection>();
    public DbSet<Departure> Departures => Set<Departure>();

    public DbSet<Seadora.Booking.Domain.Entities.Booking> Bookings => Set<Seadora.Booking.Domain.Entities.Booking>();
    public DbSet<Seadora.Booking.Domain.Entities.Feedback> Feedbacks => Set<Seadora.Booking.Domain.Entities.Feedback>();
    public DbSet<Seadora.Booking.Domain.Entities.Notification> Notifications => Set<Seadora.Booking.Domain.Entities.Notification>();
    public DbSet<Seadora.Booking.Domain.Entities.ContactInquiry> ContactInquiries => Set<Seadora.Booking.Domain.Entities.ContactInquiry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Seadora.Booking.Domain.Entities.Booking>()
            .Property(booking => booking.Status)
            .HasConversion<string>();

        modelBuilder.Entity<Seadora.Booking.Domain.Entities.Booking>()
            .Property(booking => booking.SelectedAddons)
            .HasColumnType("jsonb");

        modelBuilder.Entity<Seadora.Booking.Domain.Entities.Booking>()
            .Property(booking => booking.GuestsList)
            .HasColumnType("jsonb");

        // ponytail: owned-to-columns (Money_*), not jsonb - keeps it queryable and InMemory-materializable.
        modelBuilder.Entity<Seadora.Booking.Domain.Entities.Booking>().OwnsOne(b => b.Money);

        modelBuilder.Entity<Seadora.Booking.Domain.Entities.Notification>()
            .Property(n => n.Type)
            .HasConversion<string>();

        modelBuilder.Entity<Seadora.Booking.Domain.Entities.ContactInquiry>()
            .Property(c => c.Status)
            .HasConversion<string>();

        // ponytail: Booking both consumes (ProcessedMessages) and publishes (OutboxMessages) now.
        modelBuilder.Entity<ProcessedMessage>().HasKey(p => new { p.MessageId, p.ConsumerName });
        modelBuilder.Entity<OutboxMessage>();

        modelBuilder.Entity<TourProjection>().HasKey(p => p.TourId);
        modelBuilder.Entity<TourProjection>().Property(p => p.AllocationModel).HasConversion<string>();

        modelBuilder.Entity<Departure>(e =>
        {
            e.HasKey(d => d.Id);
            e.Property(d => d.AllocationModel).HasConversion<string>();
            // Npgsql system column xmin as the optimistic concurrency token - adds NO real column
            e.Property(d => d.Version).IsRowVersion().HasColumnName("xmin").HasColumnType("xid");
            // one departure per tour/start/slot - concurrent creators collide, one wins
            e.HasIndex(d => new { d.TourId, d.StartUtc, d.TimeSlot }).IsUnique();
        });
    }
}
