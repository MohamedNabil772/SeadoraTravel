using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Application.Common.Interfaces;
using Seadora.Content.Application.DTOs;
using System;
using System.Threading;
using System.Threading.Tasks;
using Mapster;

namespace Seadora.Content.Application.Destinations.Queries.GetDestinationById;

public record GetDestinationByIdQuery(Guid Id) : IRequest<DestinationDto?>;

public class GetDestinationByIdQueryHandler : IRequestHandler<GetDestinationByIdQuery, DestinationDto?>
{
    private readonly IContentDbContext _context;

    public GetDestinationByIdQueryHandler(IContentDbContext context)
    {
        _context = context;
    }

    public async Task<DestinationDto?> Handle(GetDestinationByIdQuery request, CancellationToken cancellationToken)
    {
        var dest = await _context.Destinations.FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);
        return dest?.Adapt<DestinationDto>();
    }
}
