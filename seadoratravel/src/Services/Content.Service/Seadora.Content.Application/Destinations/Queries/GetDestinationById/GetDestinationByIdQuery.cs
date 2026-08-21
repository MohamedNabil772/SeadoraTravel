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
        return await _context.Destinations
            .Where(d => d.Id == request.Id)
            .Select(d => new DestinationDto
            {
                Id = d.Id,
                Names = d.Names,
                Descriptions = d.Descriptions,
                Highlights = d.Highlights,
                ImageUrl = d.ImageUrl,
                FlagEmoji = d.FlagEmoji,
                TourCount = d.Tours.Count
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
