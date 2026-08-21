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

public record CreateTourCommand(
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
) : IRequest<Guid>;

public class CreateTourCommandHandler : IRequestHandler<CreateTourCommand, Guid>
{
    private readonly IContentDbContext _context;

    public CreateTourCommandHandler(IContentDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateTourCommand request, CancellationToken cancellationToken)
    {
        if (request.Names == null || request.Names.Count == 0)
            throw new ArgumentException("Tour name is required.");

        var tour = new Tour
        {
            Id = Guid.NewGuid(),
            Names = request.Names,
            Descriptions = request.Descriptions ?? new Dictionary<string, string>(),
            Highlights = request.Highlights ?? new Dictionary<string, string>(),
            Price = request.Price,
            Currency = request.Currency ?? "EUR",
            OriginalPrice = request.OriginalPrice,
            DiscountPercentage = request.DiscountPercentage,
            Duration = request.Duration ?? string.Empty,
            StartTime = request.StartTime ?? string.Empty,
            Rating = request.Rating,
            ReviewCount = request.ReviewCount,
            ImageUrl = request.ImageUrl ?? string.Empty,
            Emoji = request.Emoji ?? string.Empty,
            BgGradient = request.BgGradient ?? string.Empty,
            Badge = request.Badge ?? string.Empty,
            DestinationId = request.DestinationId,
            CategoryId = request.CategoryId,
            SupplierId = request.SupplierId,
            SupplierPercentage = request.SupplierPercentage,
            MaxAllocations = request.MaxAllocations <= 0 ? 20 : request.MaxAllocations,
            IsTopRated = request.IsTopRated,
            IsBestseller = request.IsBestseller,
            IsInHighDemand = request.IsInHighDemand,
            ReserveAndPayLater = request.ReserveAndPayLater,
            HotelPickup = request.HotelPickup,
            FreeCancellation = request.FreeCancellation,
            IsPrivateOption = request.IsPrivateOption,
            PickupTimeType = request.PickupTimeType ?? "FixedSlots",
            AvailablePickupTimes = request.AvailablePickupTimes ?? new List<string>(),
            MediaUrls = request.Media?.Select(m => m.Url).ToList() ?? new List<string>(),
            
            Packages = request.Packages?.Select(p => new TourPackage {
                Id = p.Id != Guid.Empty ? p.Id : Guid.NewGuid(),
                Titles = p.Titles ?? new Dictionary<string, string>(),
                Descriptions = p.Descriptions ?? new Dictionary<string, string>(),
                Price = p.Price,
                Badge = p.Badge ?? string.Empty,
                Features = p.Features ?? new Dictionary<string, string>()
            }).ToList() ?? new List<TourPackage>(),

            Itinerary = request.Itinerary?.Select(i => new TourItinerary {
                ItineraryType = i.ItineraryType,
                DayNumber = i.DayNumber,
                TimeString = i.TimeString,
                Titles = i.Titles ?? new Dictionary<string, string>(),
                Descriptions = i.Descriptions ?? new Dictionary<string, string>()
            }).ToList() ?? new List<TourItinerary>(),

            Inclusions = request.Inclusions?.Select(i => new TourInclusion { Names = i.Names ?? new Dictionary<string, string>() }).ToList() ?? new List<TourInclusion>(),
            Exclusions = request.Exclusions?.Select(e => new TourInclusion { Names = e.Names ?? new Dictionary<string, string>() }).ToList() ?? new List<TourInclusion>(),
            
            Faqs = request.Faqs?.Select(f => new TourFaq {
                Questions = f.Questions ?? new Dictionary<string, string>(),
                Answers = f.Answers ?? new Dictionary<string, string>()
            }).ToList() ?? new List<TourFaq>(),

            Addons = request.Addons?.Select(a => new TourAddon {
                Id = a.Id != Guid.Empty ? a.Id : Guid.NewGuid(),
                Names = a.Names ?? new Dictionary<string, string>(),
                Descriptions = a.Descriptions ?? new Dictionary<string, string>(),
                PriceEur = a.PriceEur,
                IsPerPerson = a.IsPerPerson,
                Icon = a.Icon ?? "✨",
                Category = a.Category ?? "Optional"
            }).ToList() ?? new List<TourAddon>(),

            Media = request.Media?.Select(m => new TourMedia {
                Url = m.Url,
                Captions = m.Captions ?? new Dictionary<string, string>()
            }).ToList() ?? new List<TourMedia>()
        };

        if (request.ImportantInfo != null)
        {
            tour.ImportantInformation = new ImportantInfo
            {
                WhatToBring = request.ImportantInfo.WhatToBring ?? new(),
                NotSuitableFor = request.ImportantInfo.NotSuitableFor ?? new(),
                Notes = request.ImportantInfo.Notes ?? new()
            };
        }

        _context.Tours.Add(tour);
        await _context.SaveChangesAsync(cancellationToken);

        return tour.Id;
    }
}
