using System;
using System.Collections.Generic;

namespace Seadora.Content.Application.DTOs;

public class DestinationDto
{
    public Guid Id { get; set; }
    public Dictionary<string, string> Names { get; set; } = new();
    public Dictionary<string, string> Descriptions { get; set; } = new();
    public Dictionary<string, string> Highlights { get; set; } = new();
    public string ImageUrl { get; set; } = string.Empty;
    public string FlagEmoji { get; set; } = string.Empty;
    public int TourCount { get; set; }
}
