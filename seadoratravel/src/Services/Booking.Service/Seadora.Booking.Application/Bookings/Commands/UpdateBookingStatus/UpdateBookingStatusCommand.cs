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
            // Simulate sending feedback invitation via Email and WhatsApp
            Console.WriteLine($"[NOTIFICATION - EMAIL SENT] To: {booking.CustomerEmail} | Subject: Seadora Travel - Rate your Experience | Body: Dear {booking.CustomerName}, please rate your tour at http://localhost:3000/feedback?tourId={booking.TourId}");
            if (!string.IsNullOrWhiteSpace(booking.WhatsApp))
            {
                try
                {
                    string msg = $"Shukran {booking.CustomerName}! Hope you enjoyed your tour. Please rate us: http://localhost:3000/feedback?tourId={booking.TourId}";
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
