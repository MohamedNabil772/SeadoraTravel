using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Booking.Application.Common.Interfaces;

namespace Seadora.Booking.Application.Bookings.Commands.UpdateBookingStatus;

public record UpdateBookingStatusCommand(Guid Id, string Status) : IRequest<Unit>;

public class UpdateBookingStatusCommandHandler : IRequestHandler<UpdateBookingStatusCommand, Unit>
{
    private readonly IBookingDbContext _context;

    public UpdateBookingStatusCommandHandler(IBookingDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateBookingStatusCommand request, CancellationToken cancellationToken)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

        if (booking == null)
        {
            throw new KeyNotFoundException("Booking not found.");
        }

        if (string.IsNullOrWhiteSpace(request.Status))
        {
            throw new ArgumentException("Status is required.", nameof(request.Status));
        }

        var validStatuses = new[] { "Pending", "Confirmed", "Completed", "Cancelled" };
        if (!validStatuses.Contains(request.Status, StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Invalid booking status.", nameof(request.Status));
        }

        booking.Status = request.Status;
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
