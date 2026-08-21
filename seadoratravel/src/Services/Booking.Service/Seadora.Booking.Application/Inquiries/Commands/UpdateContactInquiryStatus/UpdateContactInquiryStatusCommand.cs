using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Booking.Application.Common.Interfaces;
using Seadora.Booking.Domain.Enums;

namespace Seadora.Booking.Application.Inquiries.Commands.UpdateContactInquiryStatus;

public record UpdateContactInquiryStatusCommand(
    Guid Id,
    InquiryStatus Status,
    string? AdminNotes = null
) : IRequest;

public class UpdateContactInquiryStatusCommandHandler : IRequestHandler<UpdateContactInquiryStatusCommand>
{
    private readonly IBookingDbContext _context;

    public UpdateContactInquiryStatusCommandHandler(IBookingDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateContactInquiryStatusCommand request, CancellationToken cancellationToken)
    {
        var inquiry = await _context.ContactInquiries
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (inquiry == null)
            throw new KeyNotFoundException($"Inquiry with ID {request.Id} not found.");

        inquiry.UpdateStatus(request.Status);
        
        if (request.AdminNotes != null)
        {
            inquiry.UpdateAdminNotes(request.AdminNotes);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
