namespace Seadora.Content.Domain.Entities;

public class TourPackage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Dictionary<string, string> Titles { get; set; } = new();
    public Dictionary<string, string> Descriptions { get; set; } = new();
    public decimal Price { get; set; }
    public string Badge { get; set; } = string.Empty;
    public string Tier { get; set; } = "Standard"; // Standard, Deluxe, VIP, Private, Custom
    public int? Capacity { get; set; }
    public Dictionary<string, string> Features { get; set; } = new();
}

public class TourItinerary
{
    public string ItineraryType { get; set; } = "Time"; // "Day" | "Time"
    public int? DayNumber { get; set; }
    public string? TimeString { get; set; }
    public Dictionary<string, string> Titles { get; set; } = new();
    public Dictionary<string, string> Descriptions { get; set; } = new();
}

public class TourFaq
{
    public Dictionary<string, string> Questions { get; set; } = new();
    public Dictionary<string, string> Answers { get; set; } = new();
}

public class ImportantInfo
{
    public Dictionary<string, string> WhatToBring { get; set; } = new();
    public Dictionary<string, string> NotSuitableFor { get; set; } = new();
    public Dictionary<string, string> Notes { get; set; } = new();
}

public class TourAddon
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Dictionary<string, string> Names { get; set; } = new();
    public Dictionary<string, string>? Descriptions { get; set; } = new();
    public decimal PriceEur { get; set; }
    public bool IsPerPerson { get; set; } = false;
    public string Icon { get; set; } = "✨";
    public string Category { get; set; } = "Optional";
}

public class TourInclusion
{
    public Dictionary<string, string> Names { get; set; } = new();
}

public class TourMedia
{
    public string Url { get; set; } = string.Empty;
    public Dictionary<string, string> Captions { get; set; } = new();
}
