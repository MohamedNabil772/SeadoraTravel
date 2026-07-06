using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Booking.Application.Common.Interfaces;

namespace Seadora.Booking.Application.Bookings.Queries;

public record GetBookingByIdQuery(Guid Id) : IRequest<Seadora.Booking.Domain.Entities.Booking>;

public class GetBookingByIdQueryHandler : IRequestHandler<GetBookingByIdQuery, Seadora.Booking.Domain.Entities.Booking>
{
    private readonly IBookingDbContext _context;

    public GetBookingByIdQueryHandler(IBookingDbContext context)
    {
        _context = context;
    }

    public async Task<Seadora.Booking.Domain.Entities.Booking> Handle(GetBookingByIdQuery request, CancellationToken cancellationToken)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

        if (booking == null)
        {
            throw new KeyNotFoundException("Booking not found.");
        }

        return booking;
    }
}
