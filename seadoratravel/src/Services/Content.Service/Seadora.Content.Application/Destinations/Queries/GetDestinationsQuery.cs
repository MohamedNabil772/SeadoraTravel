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
        var destinations = await context.Destinations.ToListAsync(cancellationToken);
        return destinations.Adapt<List<DestinationDto>>();
    }
}
