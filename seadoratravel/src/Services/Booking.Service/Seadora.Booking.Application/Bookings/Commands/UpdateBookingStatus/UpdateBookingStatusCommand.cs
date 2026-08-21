using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Seadora.Booking.Application.Common.Email;
using Seadora.Booking.Application.Common.Interfaces;

namespace Seadora.Booking.Application.Bookings.Commands.UpdateBookingStatus;

public record UpdateBookingStatusCommand(Guid Id, string Status) : IRequest<Unit>;

public class UpdateBookingStatusCommandHandler : IRequestHandler<UpdateBookingStatusCommand, Unit>
{
    private readonly IBookingDbContext _context;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<UpdateBookingStatusCommandHandler> _logger;

    public UpdateBookingStatusCommandHandler(
        IBookingDbContext context,
        IEmailSender emailSender,
        ILogger<UpdateBookingStatusCommandHandler> logger)
    {
        _context = context;
        _emailSender = emailSender;
        _logger = logger;
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

        var wasConfirmed = string.Equals(booking.Status, "Confirmed", StringComparison.OrdinalIgnoreCase);
        var nowConfirmed = string.Equals(request.Status, "Confirmed", StringComparison.OrdinalIgnoreCase);

        booking.Status = request.Status;
        await _context.SaveChangesAsync(cancellationToken);

        // Send the branded confirmation + cancellation policy only on the transition
        // into Confirmed. ponytail: email failure must not fail the status update.
        if (nowConfirmed && !wasConfirmed)
        {
            try
            {
                await _emailSender.SendTemplatedAsync(
                    booking.CustomerEmail,
                    "Your Seadora Travel booking is confirmed \u2705",
                    "Booking confirmed",
                    BookingEmail.BuildConfirmationHtml(booking),
                    replyTo: ContactChannels.InfoAddress,
                    cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send confirmation email for booking {Id}.", booking.Id);
            }
        }

        return Unit.Value;
    }
}
