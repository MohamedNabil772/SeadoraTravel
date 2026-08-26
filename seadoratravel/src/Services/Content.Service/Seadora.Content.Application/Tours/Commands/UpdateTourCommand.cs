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
    Dictionary<string, string>? Names = null,
    Dictionary<string, string>? Descriptions = null,
    Dictionary<string, string>? Highlights = null,
    decimal Price = 0,
    string? Currency = null,
    decimal? OriginalPrice = null,
    decimal? DiscountPercentage = null,
    string? Duration = null,
    string? StartTime = null,
    decimal Rating = 0,
    int ReviewCount = 0,
    string? ImageUrl = null,
    string? Emoji = null,
    string? BgGradient = null,
    string? Badge = null,
    Guid DestinationId = default,
    Guid CategoryId = default,
    Guid? TourTypeId = null,
    Guid? SupplierId = null,
    decimal SupplierPercentage = 0,
    int MaxAllocations = 20,
    int? GroupMinCapacity = null,
    int? GroupMaxCapacity = null,
    bool IsTopRated = false,
    bool IsBestseller = false,
    bool IsInHighDemand = false,
    bool ReserveAndPayLater = false,
    bool HotelPickup = false,
    bool FreeCancellation = false,
    bool IsPrivateOption = false,
    List<AdminTourPackageDto>? Packages = null,
    string? PickupTimeType = null,
    List<string>? AvailablePickupTimes = null,
    List<AdminItineraryDto>? Itinerary = null,
    List<AdminInclusionDto>? Inclusions = null,
    List<AdminInclusionDto>? Exclusions = null,
    AdminImportantInfoDto? ImportantInfo = null,
    List<AdminFaqDto>? Faqs = null,
    List<AdminAddonDto>? Addons = null,
    List<AdminMediaDto>? Media = null,
    List<string>? MediaUrls = null,
    List<string>? Includes = null
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
        if (tour == null)
            throw new KeyNotFoundException($"Tour with ID {request.Id} not found.");

        tour.Names = request.Names ?? tour.Names;
        tour.Descriptions = request.Descriptions ?? tour.Descriptions;
        tour.Highlights = request.Highlights ?? tour.Highlights;
        if (request.Price > 0) tour.Price = request.Price;
        tour.Currency = request.Currency ?? tour.Currency ?? "EUR";
        if (request.OriginalPrice.HasValue) tour.OriginalPrice = request.OriginalPrice;
        if (request.DiscountPercentage.HasValue) tour.DiscountPercentage = request.DiscountPercentage;
        if (request.Duration != null) tour.Duration = request.Duration;
        if (request.StartTime != null) tour.StartTime = request.StartTime;
        if (request.Rating > 0) tour.Rating = request.Rating;
        if (request.ReviewCount > 0) tour.ReviewCount = request.ReviewCount;
        if (request.ImageUrl != null) tour.ImageUrl = request.ImageUrl;
        if (request.Emoji != null) tour.Emoji = request.Emoji;
        if (request.BgGradient != null) tour.BgGradient = request.BgGradient;
        if (request.Badge != null) tour.Badge = request.Badge;
        if (request.DestinationId != Guid.Empty) tour.DestinationId = request.DestinationId;
        if (request.CategoryId != Guid.Empty) tour.CategoryId = request.CategoryId;
        if (request.TourTypeId.HasValue) tour.TourTypeId = request.TourTypeId;
        tour.SupplierId = request.SupplierId;
        tour.SupplierPercentage = request.SupplierPercentage;
        tour.MaxAllocations = request.MaxAllocations <= 0 ? (tour.MaxAllocations <= 0 ? 20 : tour.MaxAllocations) : request.MaxAllocations;
        tour.GroupMinCapacity = request.GroupMinCapacity ?? tour.GroupMinCapacity;
        tour.GroupMaxCapacity = request.GroupMaxCapacity ?? tour.GroupMaxCapacity;
        
        tour.IsTopRated = request.IsTopRated;
        tour.IsBestseller = request.IsBestseller;
        tour.IsInHighDemand = request.IsInHighDemand;
        tour.ReserveAndPayLater = request.ReserveAndPayLater;
        tour.HotelPickup = request.HotelPickup;
        tour.FreeCancellation = request.FreeCancellation;
        tour.IsPrivateOption = request.IsPrivateOption;
        
        if (request.PickupTimeType != null) tour.PickupTimeType = request.PickupTimeType;
        if (request.AvailablePickupTimes != null) tour.AvailablePickupTimes = request.AvailablePickupTimes;
        
        if (request.Media != null && request.Media.Count > 0)
        {
            tour.Media = request.Media.Select(m => new TourMedia {
                Url = m.Url,
                Captions = m.Captions ?? new Dictionary<string, string>()
            }).ToList();
            tour.MediaUrls = request.Media.Select(m => m.Url).ToList();
        }
        else if (request.MediaUrls != null)
        {
            tour.MediaUrls = request.MediaUrls;
        }

        if (request.Includes != null)
        {
            tour.Includes = request.Includes;
        }

        if (request.Packages != null)
        {
            tour.Packages = request.Packages.Select(p => new TourPackage {
                Id = p.Id != Guid.Empty ? p.Id : Guid.NewGuid(),
                Titles = p.Titles ?? new Dictionary<string, string>(),
                Descriptions = p.Descriptions ?? new Dictionary<string, string>(),
                Price = p.Price,
                Badge = p.Badge ?? string.Empty,
                Features = p.Features ?? new Dictionary<string, string>()
            }).ToList();
        }

        if (request.Itinerary != null)
        {
            tour.Itinerary = request.Itinerary.Select(i => new TourItinerary {
                ItineraryType = i.ItineraryType,
                DayNumber = i.DayNumber,
                TimeString = i.TimeString,
                Titles = i.Titles ?? new Dictionary<string, string>(),
                Descriptions = i.Descriptions ?? new Dictionary<string, string>()
            }).ToList();
        }

        if (request.Inclusions != null)
        {
            tour.Inclusions = request.Inclusions.Select(i => new TourInclusion { Names = i.Names ?? new Dictionary<string, string>() }).ToList();
        }
        if (request.Exclusions != null)
        {
            tour.Exclusions = request.Exclusions.Select(e => new TourInclusion { Names = e.Names ?? new Dictionary<string, string>() }).ToList();
        }

        if (request.Faqs != null)
        {
            tour.Faqs = request.Faqs.Select(f => new TourFaq {
                Questions = f.Questions ?? new Dictionary<string, string>(),
                Answers = f.Answers ?? new Dictionary<string, string>()
            }).ToList();
        }

        if (request.Addons != null)
        {
            tour.Addons = request.Addons.Select(a => new TourAddon {
                Id = a.Id != Guid.Empty ? a.Id : Guid.NewGuid(),
                Names = a.Names ?? new Dictionary<string, string>(),
                Descriptions = a.Descriptions ?? new Dictionary<string, string>(),
                PriceEur = a.PriceEur,
                IsPerPerson = a.IsPerPerson,
                Icon = a.Icon ?? "✨",
                Category = a.Category ?? "Optional"
            }).ToList();
        }

        if (request.ImportantInfo != null)
        {
            tour.ImportantInformation = new ImportantInfo
            {
                WhatToBring = request.ImportantInfo.WhatToBring ?? new(),
                NotSuitableFor = request.ImportantInfo.NotSuitableFor ?? new(),
                Notes = request.ImportantInfo.Notes ?? new()
            };
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
