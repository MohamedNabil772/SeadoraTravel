using Microsoft.EntityFrameworkCore;
using Seadora.Booking.Application.Common.Interfaces;

namespace Seadora.Booking.Infrastructure.Persistence;

public class BookingDbContext : DbContext, IBookingDbContext
{
    public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options) { }

    public DbSet<Seadora.Booking.Domain.Entities.Booking> Bookings => Set<Seadora.Booking.Domain.Entities.Booking>();
    public DbSet<Seadora.Booking.Domain.Entities.Feedback> Feedbacks => Set<Seadora.Booking.Domain.Entities.Feedback>();
}
