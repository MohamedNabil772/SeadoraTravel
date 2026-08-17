using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Application.Common.Interfaces;
using Seadora.Content.Application.Tours.Models;
using Seadora.Content.Domain.Entities;

namespace Seadora.Content.Application.Tours.Queries.Admin;

public record GetAdminToursQuery : IRequest<List<AdminTourSummaryDto>>;

public record AdminTourSummaryDto(
    Guid Id,
    Dictionary<string, string> Names,
    decimal Price,
    string Currency,
    Guid DestinationId,
    Guid CategoryId
);

public class GetAdminToursQueryHandler : IRequestHandler<GetAdminToursQuery, List<AdminTourSummaryDto>>
{
    private readonly IContentDbContext _context;
    public GetAdminToursQueryHandler(IContentDbContext context) => _context = context;

    public async Task<List<AdminTourSummaryDto>> Handle(GetAdminToursQuery request, CancellationToken cancellationToken)
    {
        var tours = await _context.Tours.AsNoTracking().ToListAsync(cancellationToken);
        return tours.Select(t => new AdminTourSummaryDto(
            t.Id,
            t.Names,
            t.Price,
            t.Currency,
            t.DestinationId,
            t.CategoryId
        )).ToList();
    }
}
