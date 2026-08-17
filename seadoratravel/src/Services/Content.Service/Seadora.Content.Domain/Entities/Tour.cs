namespace Seadora.Content.Domain.Entities;

public class Tour
{
    public Guid Id { get; set; }
    
    // Localized Fields
    public Dictionary<string, string> Names { get; set; } = new();
    public Dictionary<string, string> Descriptions { get; set; } = new();
    
    public decimal Price { get; set; }
    public string Currency { get; set; } = "EUR";
    public string Duration { get; set; } = string.Empty;
    public List<string> Includes { get; set; } = new List<string>();
    public List<string> MediaUrls { get; set; } = new List<string>();
    public string ImageUrl { get; set; } = string.Empty;
    public string Emoji { get; set; } = string.Empty;
    public string BgGradient { get; set; } = string.Empty;
    public string Badge { get; set; } = string.Empty;
    
    public Guid DestinationId { get; set; }
    public Destination? Destination { get; set; }

    public Guid CategoryId { get; set; }
    public Category? Category { get; set; }

    public Guid? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }
    public decimal SupplierPercentage { get; set; }
    public int MaxAllocations { get; set; } = 20;

    // Additional Pricing & Discount Fields
    public decimal? OriginalPrice { get; set; }
    public decimal? DiscountPercentage { get; set; }
    
    // Metadata & Stats
    public string StartTime { get; set; } = string.Empty;
    public decimal Rating { get; set; }
    public int ReviewCount { get; set; }
    
    // Flags
    public bool IsTopRated { get; set; }
    public bool IsBestseller { get; set; }
    public bool IsInHighDemand { get; set; }
    
    public bool ReserveAndPayLater { get; set; } = true;
    public bool HotelPickup { get; set; } = true;
    public bool FreeCancellation { get; set; } = true;
    public bool IsPrivateOption { get; set; }

    // Tour Packages / Option Variants
    public List<TourPackage> Packages { get; set; } = new();

    // Pickup & Departure Timing Configuration
    public string PickupTimeType { get; set; } = "FixedSlots"; // "FixedSlots", "Flexible", "DriverAssigned"
    public List<string> AvailablePickupTimes { get; set; } = new() { "15:00 - 15:30 (Sunset)", "10:00 - 10:30 (Morning)" };

    // Tour Details Tabs Data
    public Dictionary<string, string> Highlights { get; set; } = new();
    public List<TourItinerary> Itinerary { get; set; } = new();
    public List<TourInclusion> Inclusions { get; set; } = new();
    public List<TourInclusion> Exclusions { get; set; } = new();
    public ImportantInfo ImportantInformation { get; set; } = new();
    public List<TourFaq> Faqs { get; set; } = new();
    public List<TourAddon> Addons { get; set; } = new();
    public List<TourMedia> Media { get; set; } = new();
}
