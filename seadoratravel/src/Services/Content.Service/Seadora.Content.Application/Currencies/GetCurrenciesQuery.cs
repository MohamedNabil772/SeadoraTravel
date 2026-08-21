using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Application.Common.Interfaces;
using Seadora.Content.Domain.Entities;

namespace Seadora.Content.Application.Currencies;

public record GetCurrenciesQuery(bool IncludeInactive = false) : IRequest<List<Currency>>;

public class GetCurrenciesQueryHandler : IRequestHandler<GetCurrenciesQuery, List<Currency>>
{
    private readonly IContentDbContext _context;

    public GetCurrenciesQueryHandler(IContentDbContext context)
    {
        _context = context;
    }

    public async Task<List<Currency>> Handle(GetCurrenciesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Currencies.AsNoTracking();
        
        if (!request.IncludeInactive)
        {
            query = query.Where(x => x.IsActive);
        }

        var list = await query.ToListAsync(cancellationToken);
        return list.OrderByDescending(c => c.IsBase).ThenBy(c => c.Code).ToList();
    }
}
