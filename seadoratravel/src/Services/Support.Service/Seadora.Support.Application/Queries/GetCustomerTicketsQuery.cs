using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Support.Application.Interfaces;
using Seadora.Support.Domain.Entities;

namespace Seadora.Support.Application.Queries;

public record GetCustomerTicketsQuery(Guid CustomerId) : IRequest<List<Ticket>>;

public class GetCustomerTicketsQueryHandler : IRequestHandler<GetCustomerTicketsQuery, List<Ticket>>
{
    private readonly ISupportDbContext _context;

    public GetCustomerTicketsQueryHandler(ISupportDbContext context)
    {
        _context = context;
    }

    public async Task<List<Ticket>> Handle(GetCustomerTicketsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Tickets
            .Include(t => t.Messages)
            .Where(t => t.CustomerId == request.CustomerId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
