using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Seadora.Booking.Application.Common.Email;
using Seadora.Booking.Application.Common.Interfaces;

namespace Seadora.Booking.Application.Contact.Commands.ReplyToContactInquiry;

public record ReplyToContactInquiryCommand(Guid Id, string Subject, string Message) : IRequest<Unit>;

public class ReplyToContactInquiryCommandHandler : IRequestHandler<ReplyToContactInquiryCommand, Unit>
{
    private readonly IBookingDbContext _context;
    private readonly IEmailSender _emailSender;

    public ReplyToContactInquiryCommandHandler(IBookingDbContext context, IEmailSender emailSender)
    {
        _context = context;
        _emailSender = emailSender;
    }

    public async Task<Unit> Handle(ReplyToContactInquiryCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Subject))
            throw new ArgumentException("Subject is required.", nameof(request.Subject));
        if (string.IsNullOrWhiteSpace(request.Message))
            throw new ArgumentException("Reply message is required.", nameof(request.Message));

        var inquiry = await _context.ContactInquiries.FindAsync(new object?[] { request.Id }, cancellationToken)
            ?? throw new KeyNotFoundException("Contact inquiry not found.");

        var body = WebUtility.HtmlEncode(request.Message).Replace("\n", "<br/>");

        // Sending is the point of a reply, so let failures surface as an error to the admin.
        // From noreply@ (single authenticated sender); Reply-To info@ so the customer's
        // reply lands in the info mailbox.
        await _emailSender.SendAsync(inquiry.Email, request.Subject, body,
            replyTo: ContactChannels.InfoAddress, cancellationToken: cancellationToken);

        inquiry.Status = "Replied";
        inquiry.ReplyMessage = request.Message.Trim();
        inquiry.RepliedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
