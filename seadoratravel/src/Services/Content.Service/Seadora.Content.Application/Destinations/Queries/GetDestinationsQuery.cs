using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Application.Common.Interfaces;
using Seadora.Content.Application.DTOs;
using Mapster;

namespace Seadora.Content.Application.Destinations.Queries;

public record GetDestinationsQuery() : IRequest<List<DestinationDto>>;

public class GetDestinationsQueryHandler(IContentDbContext context) : IRequestHandler<GetDestinationsQuery, List<DestinationDto>>
{
    public async Task<List<DestinationDto>> Handle(GetDestinationsQuery request, CancellationToken cancellationToken)
    {
        var destinations = await context.Destinations
            .Include(d => d.Tours)
            .ToListAsync(cancellationToken);
            
        var dtos = destinations.Adapt<List<DestinationDto>>();
        
        // Manual mapping for TourCount if Mapster doesn't handle it
        foreach(var dto in dtos)
        {
            var entity = destinations.First(d => d.Id == dto.Id);
            dto.TourCount = entity.Tours?.Count ?? 0;
        }
        
        return dtos;
    }
}
