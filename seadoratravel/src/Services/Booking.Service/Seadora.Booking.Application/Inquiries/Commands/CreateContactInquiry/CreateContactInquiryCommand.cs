using MediatR;
using Seadora.Booking.Application.Common.Interfaces;
using Seadora.Booking.Domain.Entities;
using Seadora.Booking.Domain.Enums;

namespace Seadora.Booking.Application.Inquiries.Commands.CreateContactInquiry;

public record CreateContactInquiryCommand(
    string FullName,
    string Email,
    string? Phone,
    string? DestinationInterest,
    string? DateOrGuests,
    string Message
) : IRequest<Guid>;

public class CreateContactInquiryCommandHandler : IRequestHandler<CreateContactInquiryCommand, Guid>
{
    private readonly IBookingDbContext _context;

    public CreateContactInquiryCommandHandler(IBookingDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateContactInquiryCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.FullName))
            throw new ArgumentException("FullName is required.");
        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ArgumentException("Email is required.");
        if (string.IsNullOrWhiteSpace(request.Message))
            throw new ArgumentException("Message is required.");

        var inquiry = new ContactInquiry(
            request.FullName,
            request.Email,
            request.Phone,
            request.DestinationInterest,
            request.DateOrGuests,
            request.Message
        );

        _context.ContactInquiries.Add(inquiry);

        var notification = Notification.Create(
            NotificationType.ContactInquiry,
            "New VIP Contact Request",
            $"Inquiry from {request.FullName} for '{request.DestinationInterest ?? "Custom Tour"}'",
            inquiry.Id.ToString()
        );

        _context.Notifications.Add(notification);

        await _context.SaveChangesAsync(cancellationToken);

        return inquiry.Id;
    }
}
