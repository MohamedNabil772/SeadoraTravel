using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Domain.Entities;
using Seadora.Content.Application.Common.Interfaces;

namespace Seadora.Content.Application.Destinations.Queries.GetDestinations;

public record GetDestinationsQuery : IRequest<List<Destination>>;

public class GetDestinationsQueryHandler : IRequestHandler<GetDestinationsQuery, List<Destination>>
{
    private readonly IContentDbContext _context;
    public GetDestinationsQueryHandler(IContentDbContext context) => _context = context;

    public async Task<List<Destination>> Handle(GetDestinationsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Destinations.ToListAsync(cancellationToken);
    }
}
