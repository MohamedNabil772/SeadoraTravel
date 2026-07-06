using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Booking.Application.Common.Interfaces;

namespace Seadora.Booking.Application.Bookings.Queries;

public record GetAllBookingsQuery(Guid? TourId, string? Status) : IRequest<List<Seadora.Booking.Domain.Entities.Booking>>;

public class GetAllBookingsQueryHandler : IRequestHandler<GetAllBookingsQuery, List<Seadora.Booking.Domain.Entities.Booking>>
{
    private readonly IBookingDbContext _context;

    public GetAllBookingsQueryHandler(IBookingDbContext context)
    {
        _context = context;
    }

    public async Task<List<Seadora.Booking.Domain.Entities.Booking>> Handle(GetAllBookingsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Bookings.AsNoTracking();

        if (request.TourId.HasValue && request.TourId.Value != Guid.Empty)
        {
            query = query.Where(b => b.TourId == request.TourId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            query = query.Where(b => b.Status == request.Status);
        }

        return await query.OrderByDescending(b => b.BookingDate).ToListAsync(cancellationToken);
    }
}
