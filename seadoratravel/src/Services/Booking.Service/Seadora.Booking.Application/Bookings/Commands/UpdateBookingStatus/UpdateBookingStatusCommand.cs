using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Seadora.Booking.Application.Common.Interfaces;
using Seadora.Booking.Domain.Enums;
using Seadora.Common.Messaging.Outbox;
using Seadora.Contracts.Events;
using System.Collections.Generic;

namespace Seadora.Booking.Application.Bookings.Commands.UpdateBookingStatus;

public record UpdateBookingStatusCommand(Guid Id, BookingStatus Status) : IRequest<Unit>;

public class UpdateBookingStatusCommandHandler : IRequestHandler<UpdateBookingStatusCommand, Unit>
{
    private readonly IBookingDbContext _context;
    private readonly IWhatsAppNotificationService _whatsAppService;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<UpdateBookingStatusCommandHandler> _logger;
    private readonly IOutboxWriter _outbox;

    public UpdateBookingStatusCommandHandler(IBookingDbContext context, IWhatsAppNotificationService whatsAppService, IEmailSender emailSender, ILogger<UpdateBookingStatusCommandHandler> logger, IOutboxWriter outbox)
    {
        _context = context;
        _whatsAppService = whatsAppService;
        _emailSender = emailSender;
        _logger = logger;
        _outbox = outbox;
    }

    public async Task<Unit> Handle(UpdateBookingStatusCommand request, CancellationToken cancellationToken)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

        if (booking == null)
        {
            throw new KeyNotFoundException("Booking not found.");
        }

        ValidateTransition(booking, request.Status);

        booking.Status = request.Status;

        if (request.Status == BookingStatus.Cancelled)
        {
            _outbox.Enqueue(new BookingCancelled
            {
                BookingId = booking.Id,
                BranchId = booking.BranchId,
                // ponytail: cancellation-policy refund amount feeds this once the refund action exists; Finance
                // reverses the accrual on cancel regardless and only posts a refund when RefundAmount > 0.
                RefundAmount = 0m,
                Currency = booking.Money?.Currency ?? "EUR",
                Reason = null
            });
        }

        await _context.SaveChangesAsync(cancellationToken);

        if (request.Status == BookingStatus.Completed)
        {
            var feedbackUrl = $"{Seadora.Booking.Application.Common.Email.ContactChannels.FeedbackBaseUrl}?tourId={booking.TourId}";
            try
            {
                var html = $@"
                <div style='font-family: Arial, sans-serif; color: #2A3F4F; max-width: 600px; margin: 0 auto; padding: 24px; background: #FFFFFF; border: 1px solid #EAE3D6; border-radius: 16px;'>
                    <h2 style='color: #06152B; font-family: Georgia, serif;'>Shukran for Travelling with Seadora!</h2>
                    <p>Dear {booking.CustomerName},</p>
                    <p>We hope you had a magical experience on your journey with us.</p>
                    <p>We would be deeply honored if you could share your feedback to help us continually elevate our services.</p>
                    <div style='margin: 24px 0; text-align: center;'>
                        <a href='{feedbackUrl}' style='display: inline-block; background: #D4AF37; color: #06152B; font-weight: bold; padding: 12px 28px; border-radius: 8px; text-decoration: none;'>Rate Your Experience</a>
                    </div>
                    <p style='font-size: 12px; color: #6B8A9A;'>Seadora Luxury Travel • Red Sea, Egypt</p>
                </div>";
                await _emailSender.SendAsync(booking.CustomerEmail, "How was your experience? — Seadora Travel", html, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send email feedback invitation for booking {BookingId}", booking.Id);
            }

            if (!string.IsNullOrWhiteSpace(booking.WhatsApp))
            {
                try
                {
                    string msg = $"Shukran {booking.CustomerName}! Hope you enjoyed your tour with Seadora Travel. Please share your feedback: {feedbackUrl}";
                    await _whatsAppService.SendCustomMessageAsync(booking.WhatsApp, msg, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to send WhatsApp feedback invitation for booking {BookingId}", booking.Id);
                }
            }
        }
        else if (request.Status == BookingStatus.Confirmed)
        {
            if (!string.IsNullOrWhiteSpace(booking.WhatsApp))
            {
                try
                {
                    await _whatsAppService.SendBookingConfirmationAsync(booking, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to send WhatsApp confirmation for booking {BookingId}", booking.Id);
                }
            }

            try
            {
                var html = Seadora.Booking.Application.Common.Email.BookingEmail.BuildConfirmationHtml(booking);
                await _emailSender.SendTemplatedAsync(booking.CustomerEmail, "Booking Confirmed - Seadora Travel", html, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send email confirmation for booking {BookingId}", booking.Id);
            }
        }

        return Unit.Value;
    }

    private static void ValidateTransition(Domain.Entities.Booking booking, BookingStatus target)
    {
        if (booking.Status == target)
        {
            return;
        }

        // Cancelled and Completed are terminal: nothing moves out of them.
        if (booking.Status is BookingStatus.Cancelled or BookingStatus.Completed)
        {
            throw new InvalidOperationException($"Cannot change status: booking is already {booking.Status} (terminal state).");
        }

        if (target is BookingStatus.Confirmed or BookingStatus.Completed)
        {
            // Validation Rule: Booking cannot be confirmed/completed until full payment is made and all customer identification/passports are provided
            if (!booking.IsPaid)
            {
                throw new InvalidOperationException($"Cannot set booking to {target}: Full payment is required.");
            }

            if (booking.MissingIdentification)
            {
                throw new InvalidOperationException($"Cannot set booking to {target}: Passenger identification or passport records are missing.");
            }
        }

        // You complete a confirmed booking, never a pending one.
        if (target == BookingStatus.Completed && booking.Status != BookingStatus.Confirmed)
        {
            throw new InvalidOperationException("Cannot complete booking: it must be Confirmed first.");
        }
    }
}
