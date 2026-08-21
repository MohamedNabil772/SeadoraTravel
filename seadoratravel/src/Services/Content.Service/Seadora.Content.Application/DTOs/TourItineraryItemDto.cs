using System.Collections.Generic;

namespace Seadora.Content.Application.DTOs;

public class TourItineraryItemDto
{
    public string ItineraryType { get; set; } = "Time"; // "Day" | "Time"
    public int? DayNumber { get; set; }
    public string? TimeString { get; set; }
    public Dictionary<string, string> Titles { get; set; } = new();
    public Dictionary<string, string> Descriptions { get; set; } = new();
}
