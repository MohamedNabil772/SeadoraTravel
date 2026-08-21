using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Seadora.Booking.Domain.Entities;
using Seadora.Booking.Application.Common.Email;
using Seadora.Booking.Application.Common.Interfaces;

namespace Seadora.Booking.Application.Contact.Commands.CreateContactInquiry;

public record CreateContactInquiryCommand(
    string FirstName,
    string LastName,
    string Email,
    string? Interest,
    string Message) : IRequest<Guid>;

public class CreateContactInquiryCommandHandler : IRequestHandler<CreateContactInquiryCommand, Guid>
{
    private static readonly System.Text.RegularExpressions.Regex EmailRegex =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.IgnoreCase);

    private readonly IBookingDbContext _context;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<CreateContactInquiryCommandHandler> _logger;

    public CreateContactInquiryCommandHandler(
        IBookingDbContext context,
        IEmailSender emailSender,
        ILogger<CreateContactInquiryCommandHandler> logger)
    {
        _context = context;
        _emailSender = emailSender;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateContactInquiryCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
            throw new ArgumentException("Name is required.");
        if (string.IsNullOrWhiteSpace(request.Email) || !EmailRegex.IsMatch(request.Email))
            throw new ArgumentException("A valid email is required.", nameof(request.Email));
        if (string.IsNullOrWhiteSpace(request.Message))
            throw new ArgumentException("Message is required.", nameof(request.Message));
        if (request.Message.Length > 5000)
            throw new ArgumentException("Message is too long.", nameof(request.Message));

        var inquiry = new ContactInquiry
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = request.Email.Trim(),
            Interest = string.IsNullOrWhiteSpace(request.Interest) ? null : request.Interest.Trim(),
            Message = request.Message.Trim(),
            CreatedAt = DateTime.UtcNow,
            Status = "New"
        };

        _context.ContactInquiries.Add(inquiry);
        await _context.SaveChangesAsync(cancellationToken);

        // ponytail: best-effort notification; a stored inquiry must not fail if SMTP is down.
        try
        {
            var name = WebUtility.HtmlEncode($"{inquiry.FirstName} {inquiry.LastName}");
            var body =
                $"<strong>New contact request</strong><br/><br/>" +
                $"<strong>From:</strong> {name} ({WebUtility.HtmlEncode(inquiry.Email)})<br/>" +
                $"<strong>Interest:</strong> {WebUtility.HtmlEncode(inquiry.Interest ?? "\u2014")}<br/><br/>" +
                WebUtility.HtmlEncode(inquiry.Message).Replace("\n", "<br/>") +
                "<br/><br/><em>Reply from the admin panel.</em>";
            await _emailSender.SendAsync(ContactChannels.InfoAddress, $"Contact request from {name}", body,
                replyTo: inquiry.Email, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send contact notification for inquiry {Id}.", inquiry.Id);
        }

        return inquiry.Id;
    }
}
