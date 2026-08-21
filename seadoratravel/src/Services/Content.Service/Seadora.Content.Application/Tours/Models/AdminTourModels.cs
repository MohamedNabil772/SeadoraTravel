using System;
using System.Collections.Generic;

namespace Seadora.Content.Application.Tours.Models;

public record AdminTourPackageDto(Guid Id, Dictionary<string, string> Titles, Dictionary<string, string> Descriptions, decimal Price, string Badge, Dictionary<string, string> Features);
public record AdminItineraryDto(string ItineraryType, int? DayNumber, string? TimeString, Dictionary<string, string> Titles, Dictionary<string, string> Descriptions);
public record AdminFaqDto(Dictionary<string, string> Questions, Dictionary<string, string> Answers);
public record AdminAddonDto(Guid Id, Dictionary<string, string> Names, Dictionary<string, string>? Descriptions, decimal PriceEur, bool IsPerPerson, string Icon, string Category);
public record AdminInclusionDto(Dictionary<string, string> Names);
public record AdminMediaDto(string Url, Dictionary<string, string> Captions);
public record AdminImportantInfoDto(Dictionary<string, string> WhatToBring, Dictionary<string, string> NotSuitableFor, Dictionary<string, string> Notes);
