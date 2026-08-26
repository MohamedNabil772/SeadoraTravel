using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Application.Common.Interfaces;
using Seadora.Content.Application.DTOs;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Mapster;

namespace Seadora.Content.Application.Tours.Queries.GetTours;

public record GetToursQuery(
    string? Search = null, 
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    string? Destination = null, 
    string? Category = null, 
    decimal? MinPrice = null, 
    decimal? MaxPrice = null,
    string Language = "en"
) : IRequest<List<TourSummaryDto>>;

public class GetToursQueryHandler : IRequestHandler<GetToursQuery, List<TourSummaryDto>>
{
    private readonly IContentDbContext _context;

    public GetToursQueryHandler(IContentDbContext context)
    {
        _context = context;
    }

    public async Task<List<TourSummaryDto>> Handle(GetToursQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Tours
            .Include(t => t.Destination)
            .Include(t => t.Category)
            .AsNoTracking();

        if (request.MinPrice.HasValue)
        {
            query = query.Where(t => t.Price >= request.MinPrice.Value);
        }

        if (request.MaxPrice.HasValue)
        {
            query = query.Where(t => t.Price <= request.MaxPrice.Value);
        }

        var tours = await query.ToListAsync(cancellationToken);
        
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var tokens = request.Search.ToLowerInvariant().Split(new[] { ' ', ',', '-', '+' }, StringSplitOptions.RemoveEmptyEntries);
            tours = tours.Where(t =>
            {
                var combinedText = string.Join(" ", 
                    (t.Names?.Values ?? Enumerable.Empty<string>())
                    .Concat(t.Descriptions?.Values ?? Enumerable.Empty<string>())
                    .Concat(t.Destination?.Names?.Values ?? Enumerable.Empty<string>())
                    .Concat(t.Category?.Names?.Values ?? Enumerable.Empty<string>())
                    .Concat(t.Includes ?? Enumerable.Empty<string>())
                ).ToLowerInvariant();

                return tokens.Any(tok => combinedText.Contains(tok));
            }).ToList();
        }

        var lang = request.Language.ToLower();

        var dtos = tours.Select(t => new TourSummaryDto
        {
            Id = t.Id,
            Slug = (t.Names != null && t.Names.ContainsKey("en")) ? t.Names["en"].ToLower().Replace(" ", "-") : t.Id.ToString(),
            Title = (t.Names != null && t.Names.ContainsKey(lang)) ? t.Names[lang] : (t.Names != null && t.Names.ContainsKey("en") ? t.Names["en"] : t.Names?.Values.FirstOrDefault() ?? string.Empty),
            Names = t.Names ?? new Dictionary<string, string>(),
            Descriptions = t.Descriptions ?? new Dictionary<string, string>(),
            Description = (t.Descriptions != null && t.Descriptions.ContainsKey(lang)) ? t.Descriptions[lang] : (t.Descriptions != null && t.Descriptions.ContainsKey("en") ? t.Descriptions["en"] : t.Descriptions?.Values.FirstOrDefault() ?? string.Empty),
            CategoryId = t.CategoryId,
            DestinationId = t.DestinationId,
            Price = t.Price,
            Currency = t.Currency ?? "EUR",
            Rating = t.Rating,
            DestinationName = t.Destination?.Names.ContainsKey(lang) == true ? t.Destination.Names[lang] : (t.Destination?.Names.ContainsKey("en") == true ? t.Destination.Names["en"] : t.Destination?.Names.Values.FirstOrDefault() ?? string.Empty),
            CategoryName = t.Category?.Names.ContainsKey(lang) == true ? t.Category.Names[lang] : (t.Category?.Names.ContainsKey("en") == true ? t.Category.Names["en"] : t.Category?.Names.Values.FirstOrDefault() ?? string.Empty),
            Images = t.MediaUrls ?? new List<string>(),
            MainImage = t.MediaUrls?.FirstOrDefault() ?? string.Empty,
            Duration = t.Duration,
            Includes = t.Includes ?? new List<string>(),
            MaxAllocations = t.MaxAllocations > 0 ? t.MaxAllocations : (t.GroupMaxCapacity ?? 20),
            GroupMinCapacity = t.GroupMinCapacity,
            GroupMaxCapacity = t.GroupMaxCapacity,
            TourTypeId = t.TourTypeId,
            SupplierId = t.SupplierId,
            SupplierPercentage = t.SupplierPercentage,
            OriginalPrice = t.OriginalPrice,
            DiscountPercentage = t.DiscountPercentage
        }).ToList();

        return dtos;
    }
}
