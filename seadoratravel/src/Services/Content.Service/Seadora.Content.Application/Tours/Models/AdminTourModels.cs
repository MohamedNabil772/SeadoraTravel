using System;
using System.Collections.Generic;

namespace Seadora.Content.Application.Tours.Models;

public record AdminItineraryDto(Dictionary<string, string> Titles, Dictionary<string, string> Descriptions, string Duration);
public record AdminFaqDto(Dictionary<string, string> Questions, Dictionary<string, string> Answers);
public record AdminAddonDto(Dictionary<string, string> Names, decimal Price);
public record AdminInclusionDto(Dictionary<string, string> Titles, bool IsIncluded);
public record AdminMediaDto(string Url, bool IsCover);
