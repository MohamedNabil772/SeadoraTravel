using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Booking.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Seadora.Booking.Application.Inquiries.Commands.ReplyToContactInquiry;

public class ReplyToContactInquiryCommandHandler : IRequestHandler<ReplyToContactInquiryCommand>
{
    private readonly IBookingDbContext _context;
    private readonly IEmailSender _emailSender;

    public ReplyToContactInquiryCommandHandler(IBookingDbContext context, IEmailSender emailSender)
    {
        _context = context;
        _emailSender = emailSender;
    }

    public async Task Handle(ReplyToContactInquiryCommand request, CancellationToken cancellationToken)
    {
        var inquiry = await _context.ContactInquiries.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        
        if (inquiry == null)
            throw new System.Collections.Generic.KeyNotFoundException($"Contact Inquiry {request.Id} not found.");

        inquiry.Reply(request.ReplyMessage);

        var subject = "Reply to your inquiry at Seadora";
        var htmlMessage = $@"
            <div style='font-family: Arial, sans-serif; color: #333;'>
                <h2>Hello {inquiry.FullName},</h2>
                <p>Thank you for reaching out to Seadora Travel.</p>
                <p>{request.ReplyMessage}</p>
                <br />
                <p>Best regards,</p>
                <p><strong>The Seadora Team</strong></p>
            </div>";

        await _emailSender.SendAsync(inquiry.Email, subject, htmlMessage, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
