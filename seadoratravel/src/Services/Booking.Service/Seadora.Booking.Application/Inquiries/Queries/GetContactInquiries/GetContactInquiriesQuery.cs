using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Booking.Application.Common.Interfaces;
using Seadora.Booking.Application.Common.Models;
using Seadora.Booking.Application.DTOs;
using Seadora.Booking.Domain.Enums;

namespace Seadora.Booking.Application.Inquiries.Queries.GetContactInquiries;

public record GetContactInquiriesQuery(
    InquiryStatus? Status = null,
    string? Search = null,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<PagedResult<ContactInquiryDto>>;

public class GetContactInquiriesQueryHandler : IRequestHandler<GetContactInquiriesQuery, PagedResult<ContactInquiryDto>>
{
    private readonly IBookingDbContext _context;

    public GetContactInquiriesQueryHandler(IBookingDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<ContactInquiryDto>> Handle(GetContactInquiriesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.ContactInquiries.AsNoTracking();

        if (request.Status.HasValue)
        {
            query = query.Where(x => x.Status == request.Status.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower();
            query = query.Where(x => 
                x.FullName.ToLower().Contains(search) || 
                x.Email.ToLower().Contains(search) || 
                (x.DestinationInterest != null && x.DestinationInterest.ToLower().Contains(search)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new ContactInquiryDto(
                x.Id,
                x.FullName,
                x.Email,
                x.Phone,
                x.DestinationInterest,
                x.DateOrGuests,
                x.Message,
                x.Status,
                x.AdminNotes,
                x.CreatedAt,
                x.UpdatedAt
            ))
            .ToListAsync(cancellationToken);

        return new PagedResult<ContactInquiryDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
