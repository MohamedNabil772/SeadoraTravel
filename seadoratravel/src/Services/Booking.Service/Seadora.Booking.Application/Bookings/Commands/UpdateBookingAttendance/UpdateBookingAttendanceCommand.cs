using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Booking.Application.Common.Interfaces;

namespace Seadora.Booking.Application.Bookings.Commands.UpdateBookingAttendance;

public record UpdateBookingAttendanceCommand(Guid Id, string Attendance) : IRequest<Unit>;

public class UpdateBookingAttendanceCommandHandler : IRequestHandler<UpdateBookingAttendanceCommand, Unit>
{
    private readonly IBookingDbContext _context;

    public UpdateBookingAttendanceCommandHandler(IBookingDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateBookingAttendanceCommand request, CancellationToken cancellationToken)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

        if (booking == null)
        {
            throw new KeyNotFoundException("Booking not found.");
        }

        booking.Attendance = request.Attendance ?? "Pending";
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
