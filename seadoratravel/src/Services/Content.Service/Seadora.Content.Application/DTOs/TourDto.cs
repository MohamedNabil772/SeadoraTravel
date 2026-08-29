using System;
using System.Collections.Generic;
using Seadora.Content.Domain.Entities;

namespace Seadora.Content.Application.DTOs;

public class TourDto
{
    public Guid Id { get; set; }
    public Dictionary<string, string> Names { get; set; } = new();
    public Dictionary<string, string> Descriptions { get; set; } = new();
    
    public decimal Price { get; set; }
    public string Currency { get; set; } = "EUR";
    public string Duration { get; set; } = string.Empty;
    public List<string> Includes { get; set; } = new();
    public List<string> MediaUrls { get; set; } = new();
    public string ImageUrl { get; set; } = string.Empty;
    public string Emoji { get; set; } = string.Empty;
    public string BgGradient { get; set; } = string.Empty;
    public string Badge { get; set; } = string.Empty;
    
    public Guid DestinationId { get; set; }
    public Guid CategoryId { get; set; }
    public Guid? TourTypeId { get; set; }
    public Guid? SupplierId { get; set; }
    public decimal SupplierPercentage { get; set; }
    public int MaxAllocations { get; set; }
    public int? GroupMinCapacity { get; set; } = 1;
    public int? GroupMaxCapacity { get; set; } = 20;

    // Additional Pricing & Discount Fields
    public decimal? OriginalPrice { get; set; }
    public decimal? DiscountPercentage { get; set; }
    
    // Metadata & Stats
    public string StartTime { get; set; } = string.Empty;
    public decimal Rating { get; set; }
    public int ReviewCount { get; set; }
    public int FavoriteCount { get; set; }
    
    // Flags
    public bool IsTopRated { get; set; }
    public bool IsBestseller { get; set; }
    public bool IsInHighDemand { get; set; }
    
    public bool ReserveAndPayLater { get; set; }
    public bool HotelPickup { get; set; }
    public bool FreeCancellation { get; set; }
    public bool IsPrivateOption { get; set; }

    // Rich Tabs Data
    public Dictionary<string, string> Highlights { get; set; } = new();
    public List<TourItineraryItemDto> Itinerary { get; set; } = new();
    public Dictionary<string, List<string>> Inclusions { get; set; } = new();
    public Dictionary<string, List<string>> Exclusions { get; set; } = new();
    public ImportantInfo ImportantInformation { get; set; } = new();
    public List<TourFaqDto> Faqs { get; set; } = new();

    // Tour Packages / Option Variants
    public List<TourPackage> Packages { get; set; } = new();

    // Pickup & Departure Timing Configuration
    public string PickupTimeType { get; set; } = "FixedSlots"; // "FixedSlots", "Flexible", "DriverAssigned"
    public List<string> AvailablePickupTimes { get; set; } = new();

    public List<TourAddon> Addons { get; set; } = new();
    public List<TourMedia> Media { get; set; } = new();

    public List<TourDateAvailabilityDto> AvailableDates { get; set; } = new();
}
