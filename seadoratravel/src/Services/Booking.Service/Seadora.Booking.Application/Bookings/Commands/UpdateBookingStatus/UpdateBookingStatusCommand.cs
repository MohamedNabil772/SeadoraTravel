using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Seadora.Booking.Application.Common.Interfaces;
using Seadora.Booking.Domain.Enums;
using System.Collections.Generic;

namespace Seadora.Booking.Application.Bookings.Commands.UpdateBookingStatus;

public record UpdateBookingStatusCommand(Guid Id, BookingStatus Status) : IRequest<Unit>;

public class UpdateBookingStatusCommandHandler : IRequestHandler<UpdateBookingStatusCommand, Unit>
{
    private readonly IBookingDbContext _context;
    private readonly IWhatsAppNotificationService _whatsAppService;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<UpdateBookingStatusCommandHandler> _logger;

    public UpdateBookingStatusCommandHandler(IBookingDbContext context, IWhatsAppNotificationService whatsAppService, IEmailSender emailSender, ILogger<UpdateBookingStatusCommandHandler> logger)
    {
        _context = context;
        _whatsAppService = whatsAppService;
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

        booking.Status = request.Status;
        await _context.SaveChangesAsync(cancellationToken);

        if (request.Status == BookingStatus.Completed)
        {
            var feedbackUrl = $"https://seadoratravel.com/feedback?tourId={booking.TourId}";
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
}
