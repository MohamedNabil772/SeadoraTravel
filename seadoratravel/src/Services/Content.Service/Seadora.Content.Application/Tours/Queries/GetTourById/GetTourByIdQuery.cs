using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Application.Common.Interfaces;
using Seadora.Content.Application.DTOs;
using System;
using System.Threading;
using System.Threading.Tasks;
using Mapster;
using Seadora.Content.Domain.Entities;

namespace Seadora.Content.Application.Tours.Queries.GetTourById;

public record GetTourByIdQuery(Guid Id) : IRequest<TourDto?>;

public class GetTourByIdQueryHandler : IRequestHandler<GetTourByIdQuery, TourDto?>
{
    private readonly IContentDbContext _context;

    public GetTourByIdQueryHandler(IContentDbContext context)
    {
        _context = context;
    }

    public async Task<TourDto?> Handle(GetTourByIdQuery request, CancellationToken cancellationToken)
    {
        var tour = await _context.Tours.AsNoTracking().FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);
        if (tour == null) return null;

        var dto = tour.Adapt<TourDto>();
        
        dto.Inclusions = MapInclusions(tour.Inclusions);
        dto.Exclusions = MapInclusions(tour.Exclusions);

        return dto;
    }

    private Dictionary<string, List<string>> MapInclusions(List<TourInclusion> inclusions)
    {
        var result = new Dictionary<string, List<string>>();
        if (inclusions == null) return result;
        foreach (var inc in inclusions)
        {
            if (inc.Names == null) continue;
            foreach (var kvp in inc.Names)
            {
                if (!result.ContainsKey(kvp.Key))
                {
                    result[kvp.Key] = new List<string>();
                }
                result[kvp.Key].Add(kvp.Value);
            }
        }
        return result;
    }
}
