using System.Collections.Generic;

namespace Seadora.Content.Application.DTOs;

public class TourFaqDto
{
    public Dictionary<string, string> Questions { get; set; } = new();
    public Dictionary<string, string> Answers { get; set; } = new();
}
