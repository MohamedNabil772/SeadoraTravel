using Microsoft.EntityFrameworkCore;

namespace Seadora.Booking.Application.Common.Interfaces;

public interface IBookingDbContext
{
    DbSet<Seadora.Booking.Domain.Entities.Booking> Bookings { get; }
    DbSet<Seadora.Booking.Domain.Entities.Feedback> Feedbacks { get; }
    DbSet<Seadora.Booking.Domain.Entities.Notification> Notifications { get; }
    DbSet<Seadora.Booking.Domain.Entities.ContactInquiry> ContactInquiries { get; }
    DbSet<Seadora.Booking.Domain.Entities.TourProjection> TourProjections { get; }
    DbSet<Seadora.Booking.Domain.Entities.Departure> Departures { get; }
    Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker ChangeTracker { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
