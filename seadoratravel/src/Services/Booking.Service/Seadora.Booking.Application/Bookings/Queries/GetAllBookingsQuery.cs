using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Booking.Application.Common.Interfaces;
using Seadora.Booking.Application.DTOs;
using Mapster;

namespace Seadora.Booking.Application.Bookings.Queries;

using Seadora.Booking.Domain.Enums;

using Seadora.Booking.Application.Common.Models;

public record GetAllBookingsQuery(
    Guid? TourId, 
    BookingStatus? Status,
    string? SortColumn,
    string? SortOrder,
    int PageNumber = 1,
    int PageSize = 10) : IRequest<PagedResult<BookingDto>>;

public class GetAllBookingsQueryHandler : IRequestHandler<GetAllBookingsQuery, PagedResult<BookingDto>>
{
    private readonly IBookingDbContext _context;

    public GetAllBookingsQueryHandler(IBookingDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<BookingDto>> Handle(GetAllBookingsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Bookings.AsNoTracking();

        if (request.TourId.HasValue && request.TourId.Value != Guid.Empty)
        {
            query = query.Where(b => b.TourId == request.TourId.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(b => b.Status == request.Status.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        // Sorting
        bool isDescending = string.Equals(request.SortOrder, "desc", StringComparison.OrdinalIgnoreCase);
        query = request.SortColumn?.ToLower() switch
        {
            "bookingdate" => isDescending ? query.OrderByDescending(b => b.BookingDate) : query.OrderBy(b => b.BookingDate),
            "tourdate" => isDescending ? query.OrderByDescending(b => b.TourDate) : query.OrderBy(b => b.TourDate),
            "customername" => isDescending ? query.OrderByDescending(b => b.CustomerName) : query.OrderBy(b => b.CustomerName),
            "totalprice" => isDescending ? query.OrderByDescending(b => b.TotalPrice) : query.OrderBy(b => b.TotalPrice),
            _ => query.OrderByDescending(b => b.BookingDate)
        };

        // Pagination
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

        var bookings = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<BookingDto>
        {
            Items = bookings.Adapt<List<BookingDto>>(),
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
