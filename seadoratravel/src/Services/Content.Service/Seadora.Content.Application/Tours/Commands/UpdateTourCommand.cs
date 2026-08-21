using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Application.Common.Interfaces;
using Seadora.Content.Domain.Entities;
using Seadora.Content.Application.Tours.Models;

namespace Seadora.Content.Application.Tours.Commands;

public record UpdateTourCommand(
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
) : IRequest<Unit>;

public class UpdateTourCommandHandler : IRequestHandler<UpdateTourCommand, Unit>
{
    private readonly IContentDbContext _context;

    public UpdateTourCommandHandler(IContentDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateTourCommand request, CancellationToken cancellationToken)
    {
        var tour = await _context.Tours.FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);
        if (tour == null) throw new KeyNotFoundException("Tour not found.");
        if (request.Names == null || request.Names.Count == 0) throw new ArgumentException("Tour name is required.");

        tour.Names = request.Names;
        tour.Descriptions = request.Descriptions ?? new Dictionary<string, string>();
        tour.Highlights = request.Highlights ?? new Dictionary<string, string>();
        tour.Price = request.Price;
        tour.Currency = request.Currency ?? "EUR";
        tour.OriginalPrice = request.OriginalPrice;
        tour.DiscountPercentage = request.DiscountPercentage;
        tour.Duration = request.Duration ?? string.Empty;
        tour.StartTime = request.StartTime ?? string.Empty;
        tour.Rating = request.Rating;
        tour.ReviewCount = request.ReviewCount;
        tour.ImageUrl = request.ImageUrl ?? string.Empty;
        tour.Emoji = request.Emoji ?? string.Empty;
        tour.BgGradient = request.BgGradient ?? string.Empty;
        tour.Badge = request.Badge ?? string.Empty;
        tour.DestinationId = request.DestinationId;
        tour.CategoryId = request.CategoryId;
        tour.SupplierId = request.SupplierId;
        tour.SupplierPercentage = request.SupplierPercentage;
        tour.MaxAllocations = request.MaxAllocations <= 0 ? 20 : request.MaxAllocations;
        
        tour.IsTopRated = request.IsTopRated;
        tour.IsBestseller = request.IsBestseller;
        tour.IsInHighDemand = request.IsInHighDemand;
        tour.ReserveAndPayLater = request.ReserveAndPayLater;
        tour.HotelPickup = request.HotelPickup;
        tour.FreeCancellation = request.FreeCancellation;
        tour.IsPrivateOption = request.IsPrivateOption;
        
        tour.PickupTimeType = request.PickupTimeType ?? "FixedSlots";
        tour.AvailablePickupTimes = request.AvailablePickupTimes ?? new List<string>();
        
        tour.MediaUrls = request.Media?.Select(m => m.Url).ToList() ?? new List<string>();

        tour.Packages = request.Packages?.Select(p => new TourPackage {
            Id = p.Id != Guid.Empty ? p.Id : Guid.NewGuid(),
            Titles = p.Titles ?? new Dictionary<string, string>(),
            Descriptions = p.Descriptions ?? new Dictionary<string, string>(),
            Price = p.Price,
            Badge = p.Badge ?? string.Empty,
            Features = p.Features ?? new Dictionary<string, string>()
        }).ToList() ?? new List<TourPackage>();

        tour.Itinerary = request.Itinerary?.Select(i => new TourItinerary {
            ItineraryType = i.ItineraryType,
            DayNumber = i.DayNumber,
            TimeString = i.TimeString,
            Titles = i.Titles ?? new Dictionary<string, string>(),
            Descriptions = i.Descriptions ?? new Dictionary<string, string>()
        }).ToList() ?? new List<TourItinerary>();

        tour.Inclusions = request.Inclusions?.Select(i => new TourInclusion { Names = i.Names ?? new Dictionary<string, string>() }).ToList() ?? new List<TourInclusion>();
        tour.Exclusions = request.Exclusions?.Select(e => new TourInclusion { Names = e.Names ?? new Dictionary<string, string>() }).ToList() ?? new List<TourInclusion>();

        tour.Faqs = request.Faqs?.Select(f => new TourFaq {
            Questions = f.Questions ?? new Dictionary<string, string>(),
            Answers = f.Answers ?? new Dictionary<string, string>()
        }).ToList() ?? new List<TourFaq>();

        tour.Addons = request.Addons?.Select(a => new TourAddon {
            Id = a.Id != Guid.Empty ? a.Id : Guid.NewGuid(),
            Names = a.Names ?? new Dictionary<string, string>(),
            Descriptions = a.Descriptions ?? new Dictionary<string, string>(),
            PriceEur = a.PriceEur,
            IsPerPerson = a.IsPerPerson,
            Icon = a.Icon ?? "✨",
            Category = a.Category ?? "Optional"
        }).ToList() ?? new List<TourAddon>();

        tour.Media = request.Media?.Select(m => new TourMedia {
            Url = m.Url,
            Captions = m.Captions ?? new Dictionary<string, string>()
        }).ToList() ?? new List<TourMedia>();

        if (request.ImportantInfo != null)
        {
            tour.ImportantInformation = new ImportantInfo
            {
                WhatToBring = request.ImportantInfo.WhatToBring ?? new(),
                NotSuitableFor = request.ImportantInfo.NotSuitableFor ?? new(),
                Notes = request.ImportantInfo.Notes ?? new()
            };
        }
        else 
        {
            tour.ImportantInformation = new ImportantInfo();
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
