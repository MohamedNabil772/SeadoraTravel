using System;
using System.Collections.Generic;

namespace Seadora.Content.Application.DTOs;

public class TourSummaryDto
{
    public Guid Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty; // Resolved title (e.g. English)
    public Dictionary<string, string> Names { get; set; } = new();
    public Dictionary<string, string> Descriptions { get; set; } = new();
    public string Description { get; set; } = string.Empty;
    public Guid? CategoryId { get; set; }
    public Guid? DestinationId { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = "EUR";
    public string FormattedPrice => $"{Price:N2} {Currency}";
    public decimal Rating { get; set; }
    public string DestinationName { get; set; } = string.Empty;
    public Dictionary<string, string> DestinationNames { get; set; } = new();
    public string CategoryName { get; set; } = string.Empty;
    public Dictionary<string, string> CategoryNames { get; set; } = new();
    public List<string> Images { get; set; } = new();
    public string MainImage { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public List<string> Includes { get; set; } = new();
    public int MaxAllocations { get; set; }
    public int? GroupMinCapacity { get; set; }
    public int? GroupMaxCapacity { get; set; }
    public Guid? TourTypeId { get; set; }
    public Guid? SupplierId { get; set; }
    public decimal SupplierPercentage { get; set; }
    public decimal? OriginalPrice { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public int FavoriteCount { get; set; }
}
