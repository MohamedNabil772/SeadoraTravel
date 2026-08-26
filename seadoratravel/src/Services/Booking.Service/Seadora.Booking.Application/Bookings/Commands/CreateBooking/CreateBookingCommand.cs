using MediatR;
using Seadora.Booking.Domain.Entities;
using Seadora.Booking.Application.Common.Interfaces;
using Seadora.Booking.Application.DTOs;
using Microsoft.Extensions.Logging;

namespace Seadora.Booking.Application.Bookings.Commands.CreateBooking;

public record CreateBookingCommand(
    Guid TourId, 
    string CustomerName, 
    string CustomerEmail,
    string? WhatsApp = null,
    string? HotelName = null,
    string? RoomNumber = null,
    string? PickupTime = null,
    string? PassportFileName = null,
    string? TripType = null,
    DateTime? TourDate = null,
    int Guests = 1,
    bool HotelPickup = false,
    Guid? PackageId = null,
    decimal TotalPrice = 0,
    string? Language = "en",
    List<BookingAddonSnapshot>? SelectedAddons = null,
    List<GuestDetailDto>? GuestsList = null
) : IRequest<Guid>;

public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, Guid>
{
    private readonly IBookingDbContext _context;
    private readonly IWhatsAppNotificationService _whatsAppService;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<CreateBookingCommandHandler> _logger;
    private static readonly System.Text.RegularExpressions.Regex EmailRegex = 
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.IgnoreCase);

    public CreateBookingCommandHandler(IBookingDbContext context, IWhatsAppNotificationService whatsAppService, IEmailSender emailSender, ILogger<CreateBookingCommandHandler> logger)
    {
        _context = context;
        _whatsAppService = whatsAppService;
        _emailSender = emailSender;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        if (request.TourId == Guid.Empty)
        {
            throw new ArgumentException("TourId cannot be empty.", nameof(request.TourId));
        }

        if (string.IsNullOrWhiteSpace(request.CustomerName))
        {
            throw new ArgumentException("CustomerName is required.", nameof(request.CustomerName));
        }

        if (request.CustomerName.Length < 2 || request.CustomerName.Length > 100)
        {
            throw new ArgumentException("CustomerName must be between 2 and 100 characters.", nameof(request.CustomerName));
        }

        if (string.IsNullOrWhiteSpace(request.CustomerEmail))
        {
            throw new ArgumentException("CustomerEmail is required.", nameof(request.CustomerEmail));
        }

        if (!EmailRegex.IsMatch(request.CustomerEmail))
        {
            throw new ArgumentException("CustomerEmail is not in a valid format.", nameof(request.CustomerEmail));
        }

        var guestsListDomain = new List<GuestDetail>();
        if (request.GuestsList != null)
        {
            foreach (var guest in request.GuestsList)
            {
                if (string.IsNullOrWhiteSpace(guest.FullName))
                {
                    throw new ArgumentException("Guest FullName is required.", nameof(request.GuestsList));
                }
                guestsListDomain.Add(new GuestDetail
                {
                    Id = Guid.NewGuid(),
                    FullName = guest.FullName,
                    PassportFileName = guest.PassportFileName,
                    AgeCategory = guest.AgeCategory,
                    Nationality = guest.Nationality,
                    SpecialRequests = guest.SpecialRequests
                });
            }
        }


        var booking = new Seadora.Booking.Domain.Entities.Booking
        {
            Id = Guid.NewGuid(),
            TourId = request.TourId,
            CustomerName = request.CustomerName,
            CustomerEmail = request.CustomerEmail,
            WhatsApp = request.WhatsApp,
            HotelName = request.HotelName,
            RoomNumber = request.RoomNumber,
            PickupTime = request.PickupTime,
            PassportFileName = request.PassportFileName,
            TripType = request.TripType,
            TourDate = request.TourDate,
            Guests = request.Guests,
            HotelPickup = request.HotelPickup,
            PackageId = request.PackageId,
            TotalPrice = request.TotalPrice,
            Language = string.IsNullOrWhiteSpace(request.Language) ? "en" : request.Language.ToLowerInvariant().Trim(),
            SelectedAddons = request.SelectedAddons ?? new(),
            GuestsList = guestsListDomain,
            BookingDate = DateTime.UtcNow,
            Status = Seadora.Booking.Domain.Enums.BookingStatus.Pending
        };

        _context.Bookings.Add(booking);
        
        var notification = Seadora.Booking.Domain.Entities.Notification.Create(
            Seadora.Booking.Domain.Enums.NotificationType.BookingCreated,
            "New VIP Tour Booking",
            $"New booking #{booking.Id.ToString().Substring(0, 8).ToUpper()} created by {request.CustomerName} for ${booking.TotalPrice:N2}",
            booking.Id.ToString()
        );
        _context.Notifications.Add(notification);

        await _context.SaveChangesAsync(cancellationToken);

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
            var html = Seadora.Booking.Application.Common.Email.BookingEmail.BuildReceiptHtml(booking);
            await _emailSender.SendAsync(booking.CustomerEmail, "Booking Received - Seadora Travel", html, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send email receipt for booking {BookingId}", booking.Id);
        }

        return booking.Id;
    }
}
