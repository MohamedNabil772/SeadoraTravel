using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Seadora.Concierge.Domain.Entities;

public class TourCatalogIndex
{
    public Guid TourId { get; set; } // PK
    public Guid BranchId { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Names { get; set; } = "{}"; // jsonb
    public string Descriptions { get; set; } = "{}"; // jsonb
    public string? DestinationName { get; set; }
    public string? CategoryName { get; set; }
    public decimal PriceEur { get; set; }
    public decimal? Rating { get; set; }
    public int? Duration { get; set; }
    public string? MainImage { get; set; }
    public bool IsActive { get; set; }
    public DateTime UpdatedUtc { get; set; }
}
