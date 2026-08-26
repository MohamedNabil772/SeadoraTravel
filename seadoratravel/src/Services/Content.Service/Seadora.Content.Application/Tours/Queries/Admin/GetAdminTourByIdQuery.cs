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

public record GetAdminTourByIdQuery(Guid Id) : IRequest<AdminTourDetailDto?>;

public record AdminTourDetailDto(
    Guid Id,
    Dictionary<string, string> Names,
    Dictionary<string, string> Descriptions,
    Dictionary<string, string> Highlights,
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
    Guid? TourTypeId,
    Guid? SupplierId,
    decimal SupplierPercentage,
    int MaxAllocations,
    int? GroupMinCapacity,
    int? GroupMaxCapacity,
    bool IsTopRated,
    bool IsBestseller,
    bool IsInHighDemand,
    bool ReserveAndPayLater,
    bool HotelPickup,
    bool FreeCancellation,
    bool IsPrivateOption,
    List<AdminTourPackageDto> Packages,
    string PickupTimeType,
    List<string> AvailablePickupTimes,
    List<AdminItineraryDto> Itinerary,
    List<AdminInclusionDto> Inclusions,
    List<AdminInclusionDto> Exclusions,
    AdminImportantInfoDto ImportantInfo,
    List<AdminFaqDto> Faqs,
    List<AdminAddonDto> Addons,
    List<AdminMediaDto> Media
);

public class GetAdminTourByIdQueryHandler : IRequestHandler<GetAdminTourByIdQuery, AdminTourDetailDto?>
{
    private readonly IContentDbContext _context;
    public GetAdminTourByIdQueryHandler(IContentDbContext context) => _context = context;

    public async Task<AdminTourDetailDto?> Handle(GetAdminTourByIdQuery request, CancellationToken cancellationToken)
    {
        var t = await _context.Tours.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (t == null) return null;

        return new AdminTourDetailDto(
            t.Id,
            t.Names ?? new Dictionary<string, string>(),
            t.Descriptions ?? new Dictionary<string, string>(),
            t.Highlights ?? new Dictionary<string, string>(),
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
            t.TourTypeId,
            t.SupplierId,
            t.SupplierPercentage,
            t.MaxAllocations,
            t.GroupMinCapacity ?? 1,
            t.GroupMaxCapacity ?? 20,
            t.IsTopRated,
            t.IsBestseller,
            t.IsInHighDemand,
            t.ReserveAndPayLater,
            t.HotelPickup,
            t.FreeCancellation,
            t.IsPrivateOption,
            t.Packages?.Select(p => new AdminTourPackageDto(p.Id, p.Titles ?? new Dictionary<string, string>(), p.Descriptions ?? new Dictionary<string, string>(), p.Price, p.Badge ?? string.Empty, p.Features ?? new Dictionary<string, string>())).ToList() ?? new List<AdminTourPackageDto>(),
            t.PickupTimeType ?? "FixedSlots",
            t.AvailablePickupTimes ?? new List<string>(),
            t.Itinerary?.Select(i => new AdminItineraryDto(i.ItineraryType, i.DayNumber, i.TimeString, i.Titles ?? new Dictionary<string, string>(), i.Descriptions ?? new Dictionary<string, string>())).ToList() ?? new List<AdminItineraryDto>(),
            t.Inclusions?.Select(inc => new AdminInclusionDto(inc.Names ?? new Dictionary<string, string>())).ToList() ?? new List<AdminInclusionDto>(),
            t.Exclusions?.Select(exc => new AdminInclusionDto(exc.Names ?? new Dictionary<string, string>())).ToList() ?? new List<AdminInclusionDto>(),
            t.ImportantInformation != null ? new AdminImportantInfoDto(t.ImportantInformation.WhatToBring ?? new(), t.ImportantInformation.NotSuitableFor ?? new(), t.ImportantInformation.Notes ?? new()) : new AdminImportantInfoDto(new(), new(), new()),
            t.Faqs?.Select(f => new AdminFaqDto(f.Questions ?? new Dictionary<string, string>(), f.Answers ?? new Dictionary<string, string>())).ToList() ?? new List<AdminFaqDto>(),
            t.Addons?.Select(a => new AdminAddonDto(a.Id, a.Names ?? new Dictionary<string, string>(), a.Descriptions ?? new Dictionary<string, string>(), a.PriceEur, a.IsPerPerson, a.Icon ?? "✨", a.Category ?? "Optional")).ToList() ?? new List<AdminAddonDto>(),
            t.Media?.Select(m => new AdminMediaDto(m.Url, m.Captions ?? new Dictionary<string, string>())).ToList() ?? new List<AdminMediaDto>()
        );
    }
}
