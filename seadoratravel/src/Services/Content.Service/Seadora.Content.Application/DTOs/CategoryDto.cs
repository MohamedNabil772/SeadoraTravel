using System;
using System.Collections.Generic;

namespace Seadora.Content.Application.DTOs;

public class CategoryDto
{
    public Guid Id { get; set; }
    public Dictionary<string, string> Names { get; set; } = new();
    public Dictionary<string, string> Descriptions { get; set; } = new();
    public string? IconName { get; set; }
    public string? CustomIconUrl { get; set; }
    public int Order { get; set; }
    public string CoverImageUrl { get; set; } = string.Empty;
    public int TourCount { get; set; }
}
