using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Booking.Application.Common.Interfaces;
using Seadora.Booking.Application.DTOs;

namespace Seadora.Booking.Application.Inquiries.Queries.GetContactInquiryById;

public record GetContactInquiryByIdQuery(Guid Id) : IRequest<ContactInquiryDto>;

public class GetContactInquiryByIdQueryHandler : IRequestHandler<GetContactInquiryByIdQuery, ContactInquiryDto>
{
    private readonly IBookingDbContext _context;

    public GetContactInquiryByIdQueryHandler(IBookingDbContext context)
    {
        _context = context;
    }

    public async Task<ContactInquiryDto> Handle(GetContactInquiryByIdQuery request, CancellationToken cancellationToken)
    {
        var inquiry = await _context.ContactInquiries
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (inquiry == null)
            throw new KeyNotFoundException($"Inquiry with ID {request.Id} not found.");

        return new ContactInquiryDto(
            inquiry.Id,
            inquiry.FullName,
            inquiry.Email,
            inquiry.Phone,
            inquiry.DestinationInterest,
            inquiry.DateOrGuests,
            inquiry.Message,
            inquiry.Status,
            inquiry.AdminNotes,
            inquiry.CreatedAt,
            inquiry.UpdatedAt
        );
    }
}
