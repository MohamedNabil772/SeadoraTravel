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

public record UpdateTourCommand(
    Guid Id,
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
) : IRequest<Unit>;

public class UpdateTourCommandHandler : IRequestHandler<UpdateTourCommand, Unit>
{
    private readonly IContentDbContext _context;

    public UpdateTourCommandHandler(IContentDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateTourCommand request, CancellationToken cancellationToken)
    {
        var tour = await _context.Tours.FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);
        if (tour == null) throw new KeyNotFoundException("Tour not found.");
        if (request.Names == null || request.Names.Count == 0) throw new ArgumentException("Tour name is required.");

        tour.Names = request.Names;
        tour.Descriptions = request.Descriptions ?? new Dictionary<string, string>();
        tour.Price = request.Price;
        tour.Currency = request.Currency ?? "EUR";
        tour.Duration = request.Duration;
        tour.DestinationId = request.DestinationId;
        tour.CategoryId = request.CategoryId;
        tour.SupplierId = request.SupplierId;
        tour.SupplierPercentage = request.SupplierPercentage;
        tour.MaxAllocations = request.MaxAllocations <= 0 ? 20 : request.MaxAllocations;
        
        tour.MediaUrls = request.Media?.Select(m => m.Url).ToList() ?? new List<string>();
        tour.ImageUrl = request.Media?.FirstOrDefault(m => m.IsCover)?.Url ?? request.Media?.FirstOrDefault()?.Url ?? string.Empty;

        tour.Itinerary = request.Itinerary?.Select(i => new TourItinerary {
            Time = i.Duration,
            Titles = i.Titles,
            Descriptions = i.Descriptions
        }).ToList() ?? new List<TourItinerary>();

        tour.Faqs = request.Faqs?.Select(f => new TourFaq {
            Questions = f.Questions,
            Answers = f.Answers
        }).ToList() ?? new List<TourFaq>();

        tour.Addons = request.Addons?.Select(a => new TourAddon {
            Id = Guid.NewGuid(),
            Names = a.Names,
            PriceEur = a.Price
        }).ToList() ?? new List<TourAddon>();

        tour.Inclusions = request.Inclusions?.Where(i => i.IsIncluded).Select(i => new TourInclusion { Names = i.Titles }).ToList() ?? new List<TourInclusion>();
        tour.Exclusions = request.Inclusions?.Where(i => !i.IsIncluded).Select(i => new TourInclusion { Names = i.Titles }).ToList() ?? new List<TourInclusion>();

        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
