using MediatR;
using Seadora.Booking.Domain.Entities;
using Seadora.Booking.Application.Common.Interfaces;

namespace Seadora.Booking.Application.Bookings.Commands.CreateBooking;

public record CreateBookingCommand(Guid TourId, string CustomerName, string CustomerEmail) : IRequest<Guid>;

public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, Guid>
{
    private readonly IBookingDbContext _context;
    private static readonly System.Text.RegularExpressions.Regex EmailRegex = 
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.IgnoreCase);

    public CreateBookingCommandHandler(IBookingDbContext context)
    {
        _context = context;
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

        var booking = new Seadora.Booking.Domain.Entities.Booking
        {
            Id = Guid.NewGuid(),
            TourId = request.TourId,
            CustomerName = request.CustomerName,
            CustomerEmail = request.CustomerEmail,
            BookingDate = DateTime.UtcNow,
            Status = "Pending"
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync(cancellationToken);

        return booking.Id;
    }
}
