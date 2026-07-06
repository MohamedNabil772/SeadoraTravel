using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Application.Common.Interfaces;
using Seadora.Content.Domain.Entities;

namespace Seadora.Content.Application.Tours.Queries.GetTourById;

public record GetTourByIdQuery(Guid Id) : IRequest<Tour>;

public class GetTourByIdQueryHandler : IRequestHandler<GetTourByIdQuery, Tour>
{
    private readonly IContentDbContext _context;

    public GetTourByIdQueryHandler(IContentDbContext context)
    {
        _context = context;
    }

    public async Task<Tour> Handle(GetTourByIdQuery request, CancellationToken cancellationToken)
    {
        var tour = await _context.Tours
            .Include(t => t.Destination)
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (tour == null)
        {
            throw new KeyNotFoundException("Tour not found.");
        }

        return tour;
    }
}
