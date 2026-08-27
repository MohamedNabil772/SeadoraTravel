using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Booking.Application.Common.Interfaces;

namespace Seadora.Booking.Application.Bookings.Commands.UpdateBookingPayment;

public record UpdateBookingPaymentCommand(Guid Id, bool IsPaid) : IRequest<Unit>;

public class UpdateBookingPaymentCommandHandler : IRequestHandler<UpdateBookingPaymentCommand, Unit>
{
    private readonly IBookingDbContext _context;

    public UpdateBookingPaymentCommandHandler(IBookingDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateBookingPaymentCommand request, CancellationToken cancellationToken)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

        if (booking == null)
        {
            throw new KeyNotFoundException("Booking not found.");
        }

        booking.IsPaid = request.IsPaid;
        // IsPaid stays the gate the confirm guard reads; Money mirrors it for the Finance phase.
        if (booking.Money is not null)
        {
            booking.Money = request.IsPaid
                ? booking.Money.WithPayment(booking.Money.Total)
                : booking.Money.WithPayment(0m);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
