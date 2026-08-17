using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Application.Common.Interfaces;
using Seadora.Content.Application.Tours.Models;
using Seadora.Content.Domain.Entities;

namespace Seadora.Content.Application.Tours.Queries.Admin;

public record GetAdminTourByIdQuery(Guid Id) : IRequest<AdminTourDetailDto?>;

public record AdminTourDetailDto(
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
);

public class GetAdminTourByIdQueryHandler : IRequestHandler<GetAdminTourByIdQuery, AdminTourDetailDto?>
{
    private readonly IContentDbContext _context;
    public GetAdminTourByIdQueryHandler(IContentDbContext context) => _context = context;

    public async Task<AdminTourDetailDto?> Handle(GetAdminTourByIdQuery request, CancellationToken cancellationToken)
    {
        var t = await _context.Tours.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (t == null) return null;

        var inclusionsList = new List<AdminInclusionDto>();
        if (t.Inclusions != null)
        {
            foreach (var inc in t.Inclusions)
            {
                inclusionsList.Add(new AdminInclusionDto(inc.Names, true));
            }
        }
        if (t.Exclusions != null)
        {
            foreach (var exc in t.Exclusions)
            {
                inclusionsList.Add(new AdminInclusionDto(exc.Names, false));
            }
        }

        var mediaList = t.MediaUrls?.Select(u => new AdminMediaDto(u, u == t.ImageUrl)).ToList() ?? new List<AdminMediaDto>();

        return new AdminTourDetailDto(
            t.Id,
            t.Names ?? new Dictionary<string, string>(),
            t.Descriptions ?? new Dictionary<string, string>(),
            t.Price,
            t.Currency ?? "EUR",
            t.Duration,
            t.DestinationId,
            t.CategoryId,
            t.SupplierId,
            t.SupplierPercentage,
            t.MaxAllocations,
            t.Itinerary?.Select(i => new AdminItineraryDto(i.Titles, i.Descriptions, i.Time)).ToList() ?? new List<AdminItineraryDto>(),
            t.Faqs?.Select(f => new AdminFaqDto(f.Questions, f.Answers)).ToList() ?? new List<AdminFaqDto>(),
            t.Addons?.Select(a => new AdminAddonDto(a.Names, a.PriceEur)).ToList() ?? new List<AdminAddonDto>(),
            inclusionsList,
            mediaList
        );
    }
}
