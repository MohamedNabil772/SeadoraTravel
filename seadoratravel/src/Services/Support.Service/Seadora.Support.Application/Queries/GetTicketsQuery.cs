using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Support.Application.Interfaces;
using Seadora.Support.Domain.Entities;
using System.Linq;

namespace Seadora.Support.Application.Queries;

public record GetTicketsQuery() : IRequest<List<TicketDto>>;

public record TicketDto(Guid Id, string Subject, string CustomerName, string Status, string Priority, DateTime CreatedAt);

public class GetTicketsQueryHandler : IRequestHandler<GetTicketsQuery, List<TicketDto>>
{
    private readonly ISupportDbContext _context;

    public GetTicketsQueryHandler(ISupportDbContext context)
    {
        _context = context;
    }

    public async Task<List<TicketDto>> Handle(GetTicketsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Tickets
            .Select(t => new TicketDto(t.Id, t.Subject, t.CustomerName, t.Status.ToString(), t.Priority.ToString(), t.CreatedAt))
            .ToListAsync(cancellationToken);
    }
}
