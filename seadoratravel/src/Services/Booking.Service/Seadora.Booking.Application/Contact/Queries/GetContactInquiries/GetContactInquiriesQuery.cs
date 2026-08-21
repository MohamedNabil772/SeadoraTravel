using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Booking.Domain.Entities;
using Seadora.Booking.Application.Common.Interfaces;

namespace Seadora.Booking.Application.Contact.Queries.GetContactInquiries;

public record GetContactInquiriesQuery : IRequest<List<ContactInquiry>>;

public class GetContactInquiriesQueryHandler : IRequestHandler<GetContactInquiriesQuery, List<ContactInquiry>>
{
    private readonly IBookingDbContext _context;

    public GetContactInquiriesQueryHandler(IBookingDbContext context)
    {
        _context = context;
    }

    public async Task<List<ContactInquiry>> Handle(GetContactInquiriesQuery request, CancellationToken cancellationToken)
    {
        return await _context.ContactInquiries
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
