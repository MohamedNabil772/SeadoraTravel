using System.Collections.Generic;

namespace Seadora.Content.Application.DTOs;

public class TourItineraryItemDto
{
    public string Time { get; set; } = string.Empty;
    public Dictionary<string, string> Titles { get; set; } = new();
    public Dictionary<string, string> Descriptions { get; set; } = new();
}
