using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Application.Common.Interfaces;
using Seadora.Content.Application.DTOs;
using Mapster;

namespace Seadora.Content.Application.Destinations.Queries;

public record GetDestinationByIdQuery(Guid Id) : IRequest<DestinationDto>;

public class GetDestinationByIdQueryHandler(IContentDbContext context) : IRequestHandler<GetDestinationByIdQuery, DestinationDto>
{
    public async Task<DestinationDto> Handle(GetDestinationByIdQuery request, CancellationToken cancellationToken)
    {
        var destination = await context.Destinations
            .Include(d => d.Tours)
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);
            
        if (destination == null) throw new KeyNotFoundException("Destination not found");
        
        var dto = destination.Adapt<DestinationDto>();
        dto.TourCount = destination.Tours?.Count ?? 0;
        return dto;
    }
}
