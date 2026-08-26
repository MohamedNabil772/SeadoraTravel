using Microsoft.EntityFrameworkCore;
using Seadora.Booking.Application.Common.Interfaces;
using Seadora.Booking.Domain.Entities;
using Seadora.Common.Messaging.Idempotency;

namespace Seadora.Booking.Infrastructure.Persistence;

public class BookingDbContext : DbContext, IBookingDbContext, IProcessedMessageDbContext
{
    public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options) { }

    public DbSet<ProcessedMessage> ProcessedMessages => Set<ProcessedMessage>();
    public DbSet<TourProjection> TourProjections => Set<TourProjection>();

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

        modelBuilder.Entity<Seadora.Booking.Domain.Entities.Notification>()
            .Property(n => n.Type)
            .HasConversion<string>();

        modelBuilder.Entity<Seadora.Booking.Domain.Entities.ContactInquiry>()
            .Property(c => c.Status)
            .HasConversion<string>();

        // ponytail: Booking is consumer-only here, so no OutboxMessage mapping (YAGNI until it publishes)
        modelBuilder.Entity<ProcessedMessage>().HasKey(p => new { p.MessageId, p.ConsumerName });

        modelBuilder.Entity<TourProjection>().HasKey(p => p.TourId);
        modelBuilder.Entity<TourProjection>().Property(p => p.AllocationModel).HasConversion<string>();
    }
}
