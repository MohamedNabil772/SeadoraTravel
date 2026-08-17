using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Application.Common.Interfaces;
using Seadora.Content.Domain.Entities;
using Seadora.Content.Application.Tours.Models;

namespace Seadora.Content.Application.Tours.Commands;

public record CreateTourCommand(
    Dictionary<string, string> Names,
    Dictionary<string, string> Descriptions,
    decimal Price,
    string Currency,
    string Duration,
    Guid DestinationId,
    Guid CategoryId,
    Guid? SupplierId,
    decimal SupplierPercentage,
    int MaxAllocations,
    List<AdminItineraryDto> Itinerary,
    List<AdminFaqDto> Faqs,
    List<AdminAddonDto> Addons,
    List<AdminInclusionDto> Inclusions,
    List<AdminMediaDto> Media
) : IRequest<Guid>;

public class CreateTourCommandHandler : IRequestHandler<CreateTourCommand, Guid>
{
    private readonly IContentDbContext _context;

    public CreateTourCommandHandler(IContentDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateTourCommand request, CancellationToken cancellationToken)
    {
        if (request.Names == null || request.Names.Count == 0)
            throw new ArgumentException("Tour name is required.");

        var tour = new Tour
        {
            Id = Guid.NewGuid(),
            Names = request.Names,
            Descriptions = request.Descriptions ?? new Dictionary<string, string>(),
            Price = request.Price,
            Currency = request.Currency ?? "EUR",
            Duration = request.Duration,
            DestinationId = request.DestinationId,
            CategoryId = request.CategoryId,
            SupplierId = request.SupplierId,
            SupplierPercentage = request.SupplierPercentage,
            MaxAllocations = request.MaxAllocations <= 0 ? 20 : request.MaxAllocations,
            MediaUrls = request.Media?.Select(m => m.Url).ToList() ?? new List<string>(),
            ImageUrl = request.Media?.FirstOrDefault(m => m.IsCover)?.Url ?? request.Media?.FirstOrDefault()?.Url ?? string.Empty,
            Itinerary = request.Itinerary?.Select(i => new TourItinerary {
                Time = i.Duration,
                Titles = i.Titles,
                Descriptions = i.Descriptions
            }).ToList() ?? new List<TourItinerary>(),
            Faqs = request.Faqs?.Select(f => new TourFaq {
                Questions = f.Questions,
                Answers = f.Answers
            }).ToList() ?? new List<TourFaq>(),
            Addons = request.Addons?.Select(a => new TourAddon {
                Id = Guid.NewGuid(),
                Names = a.Names,
                PriceEur = a.Price
            }).ToList() ?? new List<TourAddon>()
        };

        tour.Inclusions = request.Inclusions?.Where(i => i.IsIncluded).Select(i => new TourInclusion { Names = i.Titles }).ToList() ?? new List<TourInclusion>();
        tour.Exclusions = request.Inclusions?.Where(i => !i.IsIncluded).Select(i => new TourInclusion { Names = i.Titles }).ToList() ?? new List<TourInclusion>();

        _context.Tours.Add(tour);
        await _context.SaveChangesAsync(cancellationToken);

        return tour.Id;
    }
}
