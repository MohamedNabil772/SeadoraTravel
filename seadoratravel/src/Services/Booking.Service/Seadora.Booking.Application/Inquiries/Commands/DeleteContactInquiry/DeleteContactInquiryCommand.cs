using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Booking.Application.Common.Interfaces;

namespace Seadora.Booking.Application.Inquiries.Commands.DeleteContactInquiry;

public record DeleteContactInquiryCommand(Guid Id) : IRequest;

public class DeleteContactInquiryCommandHandler : IRequestHandler<DeleteContactInquiryCommand>
{
    private readonly IBookingDbContext _context;

    public DeleteContactInquiryCommandHandler(IBookingDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteContactInquiryCommand request, CancellationToken cancellationToken)
    {
        var inquiry = await _context.ContactInquiries
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (inquiry == null)
            throw new KeyNotFoundException($"Inquiry with ID {request.Id} not found.");

        _context.ContactInquiries.Remove(inquiry);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
