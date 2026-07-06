using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Application.Common.Interfaces;
using Seadora.Content.Domain.Entities;

namespace Seadora.Content.Application.Destinations.Queries.GetDestinationById;

public record GetDestinationByIdQuery(Guid Id) : IRequest<Destination>;

public class GetDestinationByIdQueryHandler : IRequestHandler<GetDestinationByIdQuery, Destination>
{
    private readonly IContentDbContext _context;

    public GetDestinationByIdQueryHandler(IContentDbContext context)
    {
        _context = context;
    }

    public async Task<Destination> Handle(GetDestinationByIdQuery request, CancellationToken cancellationToken)
    {
        var destination = await _context.Destinations
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

        if (destination == null)
        {
            throw new KeyNotFoundException("Destination not found.");
        }

        return destination;
    }
}
