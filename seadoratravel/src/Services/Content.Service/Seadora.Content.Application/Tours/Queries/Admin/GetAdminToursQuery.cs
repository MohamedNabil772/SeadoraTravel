using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Application.Common.Interfaces;
using Seadora.Content.Application.Tours.Models;
using Seadora.Content.Domain.Entities;

namespace Seadora.Content.Application.Tours.Queries.Admin;

public record GetAdminToursQuery : IRequest<List<AdminTourSummaryDto>>;

public record AdminTourSummaryDto(
    Guid Id,
    Dictionary<string, string> Names,
    decimal Price,
    string Currency,
    decimal? OriginalPrice,
    decimal? DiscountPercentage,
    string Duration,
    string StartTime,
    decimal Rating,
    int ReviewCount,
    string ImageUrl,
    string Emoji,
    string BgGradient,
    string Badge,
    Guid DestinationId,
    Guid CategoryId,
    Guid? SupplierId,
    decimal SupplierPercentage,
    int MaxAllocations,
    bool IsTopRated,
    bool IsBestseller,
    bool IsInHighDemand,
    bool ReserveAndPayLater,
    bool HotelPickup,
    bool FreeCancellation,
    bool IsPrivateOption,
    string PickupTimeType
);

public class GetAdminToursQueryHandler : IRequestHandler<GetAdminToursQuery, List<AdminTourSummaryDto>>
{
    private readonly IContentDbContext _context;
    public GetAdminToursQueryHandler(IContentDbContext context) => _context = context;

    public async Task<List<AdminTourSummaryDto>> Handle(GetAdminToursQuery request, CancellationToken cancellationToken)
    {
        var tours = await _context.Tours.AsNoTracking().ToListAsync(cancellationToken);
        return tours.Select(t => new AdminTourSummaryDto(
            t.Id,
            t.Names ?? new Dictionary<string, string>(),
            t.Price,
            t.Currency ?? "EUR",
            t.OriginalPrice,
            t.DiscountPercentage,
            t.Duration ?? string.Empty,
            t.StartTime ?? string.Empty,
            t.Rating,
            t.ReviewCount,
            t.ImageUrl ?? string.Empty,
            t.Emoji ?? string.Empty,
            t.BgGradient ?? string.Empty,
            t.Badge ?? string.Empty,
            t.DestinationId,
            t.CategoryId,
            t.SupplierId,
            t.SupplierPercentage,
            t.MaxAllocations,
            t.IsTopRated,
            t.IsBestseller,
            t.IsInHighDemand,
            t.ReserveAndPayLater,
            t.HotelPickup,
            t.FreeCancellation,
            t.IsPrivateOption,
            t.PickupTimeType ?? "FixedSlots"
        )).ToList();
    }
}
