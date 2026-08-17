using MediatR;
using Seadora.Content.Application.Common.Interfaces;
using Seadora.Content.Application.DTOs;
using Mapster;

namespace Seadora.Content.Application.Destinations.Queries;

public record GetDestinationByIdQuery(Guid Id) : IRequest<DestinationDto>;

public class GetDestinationByIdQueryHandler(IContentDbContext context) : IRequestHandler<GetDestinationByIdQuery, DestinationDto>
{
    public async Task<DestinationDto> Handle(GetDestinationByIdQuery request, CancellationToken cancellationToken)
    {
        var destination = await context.Destinations.FindAsync(new object[] { request.Id }, cancellationToken);
        if (destination == null) throw new KeyNotFoundException("Destination not found");
        
        return destination.Adapt<DestinationDto>();
    }
}
