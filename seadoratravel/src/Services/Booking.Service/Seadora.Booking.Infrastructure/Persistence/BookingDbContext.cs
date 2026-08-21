using Microsoft.EntityFrameworkCore;
using Seadora.Booking.Application.Common.Interfaces;

namespace Seadora.Booking.Infrastructure.Persistence;

public class BookingDbContext : DbContext, IBookingDbContext
{
    public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options) { }

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
    }
}
