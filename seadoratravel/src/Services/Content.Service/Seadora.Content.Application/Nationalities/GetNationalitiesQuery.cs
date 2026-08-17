using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Application.Common.Interfaces;
using Seadora.Content.Domain.Entities;

namespace Seadora.Content.Application.Nationalities;

public record GetNationalitiesQuery(bool IncludeInactive = false) : IRequest<List<Nationality>>;

public class GetNationalitiesQueryHandler : IRequestHandler<GetNationalitiesQuery, List<Nationality>>
{
    private readonly IContentDbContext _context;

    public GetNationalitiesQueryHandler(IContentDbContext context)
    {
        _context = context;
    }

    public async Task<List<Nationality>> Handle(GetNationalitiesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Nationalities.AsNoTracking();
        
        if (!request.IncludeInactive)
        {
            query = query.Where(x => x.IsActive);
        }

        return await query.ToListAsync(cancellationToken);
    }
}
