using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Domain.Entities;
using Seadora.Content.Application.Common.Interfaces;

namespace Seadora.Content.Application.Tours.Queries.GetTours;

public record GetToursQuery : IRequest<List<Tour>>;

public class GetToursQueryHandler : IRequestHandler<GetToursQuery, List<Tour>>
{
    private readonly IContentDbContext _context;
    public GetToursQueryHandler(IContentDbContext context) => _context = context;

    public async Task<List<Tour>> Handle(GetToursQuery request, CancellationToken cancellationToken)
    {
        return await _context.Tours.Include(t => t.Destination).ToListAsync(cancellationToken);
    }
}
