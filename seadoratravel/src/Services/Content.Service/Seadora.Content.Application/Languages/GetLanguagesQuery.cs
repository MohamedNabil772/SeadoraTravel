using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Application.Common.Interfaces;
using Seadora.Content.Domain.Entities;

namespace Seadora.Content.Application.Languages;

public record GetLanguagesQuery(bool IncludeInactive = false) : IRequest<List<Language>>;

public class GetLanguagesQueryHandler : IRequestHandler<GetLanguagesQuery, List<Language>>
{
    private readonly IContentDbContext _context;

    public GetLanguagesQueryHandler(IContentDbContext context)
    {
        _context = context;
    }

    public async Task<List<Language>> Handle(GetLanguagesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Languages.AsNoTracking();
        
        if (!request.IncludeInactive)
        {
            query = query.Where(x => x.IsActive);
        }

        return await query.ToListAsync(cancellationToken);
    }
}
