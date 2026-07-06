using Microsoft.EntityFrameworkCore;

namespace Seadora.Booking.Application.Common.Interfaces;

public interface IBookingDbContext
{
    DbSet<Seadora.Booking.Domain.Entities.Booking> Bookings { get; }
    DbSet<Seadora.Booking.Domain.Entities.Feedback> Feedbacks { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
