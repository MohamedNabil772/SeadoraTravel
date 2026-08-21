using Microsoft.EntityFrameworkCore;

namespace Seadora.Booking.Application.Common.Interfaces;

public interface IBookingDbContext
{
    DbSet<Seadora.Booking.Domain.Entities.Booking> Bookings { get; }
    DbSet<Seadora.Booking.Domain.Entities.Feedback> Feedbacks { get; }
    DbSet<Seadora.Booking.Domain.Entities.ContactInquiry> ContactInquiries { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
