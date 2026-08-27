using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Booking.Domain.Entities;
using Seadora.Booking.Domain.ValueObjects;
using Seadora.Booking.Application.Common.Interfaces;
using Seadora.Booking.Application.DTOs;
using Seadora.Common.Tenancy;
using Seadora.Contracts.Enums;
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
    bool MissingIdentification = false,
    List<BookingAddonSnapshot>? SelectedAddons = null,
    List<GuestDetailDto>? GuestsList = null
) : IRequest<Guid>;

public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, Guid>
{
    private readonly IBookingDbContext _context;
    private readonly IWhatsAppNotificationService _whatsAppService;
    private readonly IEmailSender _emailSender;
    private readonly ICurrentBranch _currentBranch;
    private readonly ILogger<CreateBookingCommandHandler> _logger;
    private static readonly System.Text.RegularExpressions.Regex EmailRegex = 
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.IgnoreCase);

    public CreateBookingCommandHandler(IBookingDbContext context, IWhatsAppNotificationService whatsAppService, IEmailSender emailSender, ICurrentBranch currentBranch, ILogger<CreateBookingCommandHandler> logger)
    {
        _context = context;
        _whatsAppService = whatsAppService;
        _emailSender = emailSender;
        _currentBranch = currentBranch;
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

        if (request.TotalPrice < 0)
        {
            throw new InvalidOperationException("TotalPrice cannot be negative.");
        }

        var guestsListDomain = new List<GuestDetail>();
        bool hasMissingId = false;

        if (request.GuestsList != null && request.GuestsList.Count > 0)
        {
            foreach (var guest in request.GuestsList)
            {
                if (string.IsNullOrWhiteSpace(guest.FullName))
                {
                    throw new ArgumentException("Guest FullName is required.", nameof(request.GuestsList));
                }

                bool guestHasId = !string.IsNullOrWhiteSpace(guest.PassportFileName) || !string.IsNullOrWhiteSpace(guest.PassportNumber);
                if (!guestHasId)
                {
                    hasMissingId = true;
                }

                guestsListDomain.Add(new GuestDetail
                {
                    Id = Guid.NewGuid(),
                    FullName = guest.FullName,
                    Email = guest.Email,
                    Phone = guest.Phone,
                    PassportNumber = guest.PassportNumber,
                    PassportFileName = guest.PassportFileName,
                    AgeCategory = guest.AgeCategory ?? "Adult",
                    Nationality = guest.Nationality,
                    SpecialRequests = guest.SpecialRequests
                });
            }
        }
        else
        {
            // If no individual guest breakdown provided and main passport missing, mark as missing identification
            if (string.IsNullOrWhiteSpace(request.PassportFileName))
            {
                hasMissingId = true;
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
            // Detailed guest records are authoritative: the stored count must match them.
            Guests = guestsListDomain.Count > 0 ? guestsListDomain.Count : (request.Guests > 0 ? request.Guests : 1),
            HotelPickup = request.HotelPickup,
            PackageId = request.PackageId,
            TotalPrice = request.TotalPrice,
            Language = string.IsNullOrWhiteSpace(request.Language) ? "en" : request.Language.ToLowerInvariant().Trim(),
            MissingIdentification = request.MissingIdentification || hasMissingId,
            SelectedAddons = request.SelectedAddons ?? new(),
            GuestsList = guestsListDomain,
            BookingDate = DateTime.UtcNow,
            Status = Seadora.Booking.Domain.Enums.BookingStatus.Pending
        };

        // CROSS-SERVICE BOUNDARY: TotalPrice is client-supplied; server-side price recomputation still
        // needs the Content service's package/addon pricing. Capacity is enforced below via Departures.
        var notification = Seadora.Booking.Domain.Entities.Notification.Create(
            Seadora.Booking.Domain.Enums.NotificationType.BookingCreated,
            "New VIP Tour Booking",
            $"New booking #{booking.Id.ToString().Substring(0, 8).ToUpper()} created by {request.CustomerName} for ${booking.TotalPrice:N2}",
            booking.Id.ToString()
        );

        // Departure is resolved server-side from what the website already posts - no new required input.
        var startUtc = DateTime.SpecifyKind((request.TourDate ?? DateTime.UtcNow).Date, DateTimeKind.Utc);
        var nextUtc = startUtc.AddDays(1);
        var slot = request.PickupTime ?? "";
        var guests = booking.Guests;

        var projection = await _context.TourProjections
            .FirstOrDefaultAsync(p => p.TourId == request.TourId, cancellationToken);

        booking.BranchId = _currentBranch.BranchId;
        booking.TourTypeCode = projection?.TourTypeCode; // snapshot; null for legacy tours with no projection

        // ponytail: discount/tax are 0 until the pricing engine lands - the breakdown only claims what the
        // client posted, so Money.Total == request.TotalPrice and nothing downstream shifts.
        var addonsTotal = booking.SelectedAddons?.Sum(a => a.TotalPrice) ?? 0m;
        var subtotal = Math.Max(0m, request.TotalPrice - addonsTotal);
        booking.Money = Money.Create(subtotal, addonsTotal, discount: 0m, taxTotal: 0m,
            projection?.Currency ?? "EUR");

        await CommitWithRetryAsync(async () =>
        {
            var departure = await _context.Departures.FirstOrDefaultAsync(
                d => d.TourId == request.TourId && d.StartUtc == startUtc && d.TimeSlot == slot, cancellationToken);

            var isNewDeparture = departure is null;
            if (departure is null)
            {
                departure = new Departure
                {
                    Id = Guid.NewGuid(),
                    BranchId = _currentBranch.BranchId,
                    TourId = request.TourId,
                    StartUtc = startUtc,
                    TimeSlot = slot,
                    // ponytail: unknown catalog => unbounded, don't block legacy tours with no projection.
                    Capacity = projection?.MaxCapacity ?? int.MaxValue,
                    AllocationModel = projection?.AllocationModel ?? AllocationModel.Shared
                };
                _context.Departures.Add(departure);
            }

            // ponytail: load matched by TourDate day, not a DepartureId FK - keeps the existing Booking
            // schema and the public website contract untouched. Add the FK only if slots ever split a day.
            var existingGuests = await _context.Bookings
                .Where(b => b.TourId == request.TourId
                    && b.TourDate.HasValue && b.TourDate.Value >= startUtc && b.TourDate.Value < nextUtc
                    && b.Status != Seadora.Booking.Domain.Enums.BookingStatus.Cancelled)
                .SumAsync(b => b.Guests, cancellationToken);

            if (departure.AllocationModel == AllocationModel.WholeUnit && existingGuests > 0)
            {
                throw new InvalidOperationException("This departure is already reserved (whole-unit).");
            }

            if ((long)existingGuests + guests > departure.Capacity)
            {
                throw new InvalidOperationException($"Not enough capacity: {departure.Capacity - existingGuests} seats remain, {guests} requested.");
            }

            _context.Bookings.Add(booking);
            _context.Notifications.Add(notification);
            if (!isNewDeparture)
            {
                // touch the row so its xmin token is checked in this same commit
                _context.Departures.Update(departure);
            }

            await _context.SaveChangesAsync(cancellationToken);
        }, () => _context.ChangeTracker.Clear());

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

    // ponytail: retry only transient collisions. Capacity rejections are terminal and pass straight through.
    // The real parallel-safety guarantee (Departure.xmin token + the unique TourId/StartUtc/TimeSlot index)
    // is DB-level and only provable against Postgres; InMemory tests cover the wiring, not the guarantee.
    public static async Task CommitWithRetryAsync(Func<Task> attempt, Action reset, int maxAttempts = 3)
    {
        for (var i = 1; ; i++)
        {
            try
            {
                await attempt();
                return;
            }
            catch (DbUpdateException ex) when (ex is DbUpdateConcurrencyException || IsUniqueViolation(ex))
            {
                if (i >= maxAttempts)
                {
                    throw new InvalidOperationException("Booking could not be completed due to high demand, please retry.", ex);
                }
                reset();
            }
        }
    }

    // ponytail: duck-typed SqlState beats pulling an Npgsql reference into Application for one comparison.
    private static bool IsUniqueViolation(DbUpdateException ex) =>
        ex.InnerException?.GetType().GetProperty("SqlState")?.GetValue(ex.InnerException) as string == "23505";
}
