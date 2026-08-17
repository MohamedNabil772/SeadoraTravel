using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Booking.Application.Common.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Seadora.Booking.Application.Bookings.Queries;

public record GetTourAvailabilityQuery(Guid TourId, DateTime Date) : IRequest<int>;

public class GetTourAvailabilityQueryHandler : IRequestHandler<GetTourAvailabilityQuery, int>
{
    private readonly IBookingDbContext _context;

    public GetTourAvailabilityQueryHandler(IBookingDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(GetTourAvailabilityQuery request, CancellationToken cancellationToken)
    {
        var totalBookedGuests = await _context.Bookings
            .Where(b => b.TourId == request.TourId && b.TourDate.HasValue && b.TourDate.Value.Date == request.Date.Date)
            .Where(b => b.Status != Domain.Enums.BookingStatus.Cancelled)
            .SumAsync(b => b.Guests, cancellationToken);
            
        return totalBookedGuests;
    }
}
