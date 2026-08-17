using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Application.Common.Interfaces;
using Seadora.Content.Application.DTOs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Mapster;

namespace Seadora.Content.Application.Destinations.Queries.GetDestinations;

public record GetDestinationsQuery : IRequest<List<DestinationDto>>;

public class GetDestinationsQueryHandler : IRequestHandler<GetDestinationsQuery, List<DestinationDto>>
{
    private readonly IContentDbContext _context;

    public GetDestinationsQueryHandler(IContentDbContext context)
    {
        _context = context;
    }

    public async Task<List<DestinationDto>> Handle(GetDestinationsQuery request, CancellationToken cancellationToken)
    {
        var dests = await _context.Destinations.ToListAsync(cancellationToken);
        return dests.Adapt<List<DestinationDto>>();
    }
}
