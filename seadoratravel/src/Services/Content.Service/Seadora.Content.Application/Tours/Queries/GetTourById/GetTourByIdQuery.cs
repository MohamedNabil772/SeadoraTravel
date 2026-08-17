using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Application.Common.Interfaces;
using Seadora.Content.Application.DTOs;
using System;
using System.Threading;
using System.Threading.Tasks;
using Mapster;

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
        return tour?.Adapt<TourDto>();
    }
}
