using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Support.Application.Interfaces;
using Seadora.Support.Domain.Entities;
using System.Linq;

namespace Seadora.Support.Application.Queries;

public record GetTicketByIdQuery(Guid Id) : IRequest<TicketDetailDto>;

public record TicketDetailDto(Guid Id, string Subject, string CustomerName, string CustomerEmail, string Status, string Priority, Guid? AssignedAgentId, DateTime CreatedAt, System.Collections.Generic.List<TicketMessageDto> Messages);
public record TicketMessageDto(Guid Id, string Sender, bool IsFromAgent, string Body, DateTime SentAt);

public class GetTicketByIdQueryHandler : IRequestHandler<GetTicketByIdQuery, TicketDetailDto>
{
    private readonly ISupportDbContext _context;

    public GetTicketByIdQueryHandler(ISupportDbContext context)
    {
        _context = context;
    }

    public async Task<TicketDetailDto> Handle(GetTicketByIdQuery request, CancellationToken cancellationToken)
    {
        var ticket = await _context.Tickets
            .Include(t => t.Messages)
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);
            
        if (ticket == null) return null!;

        return new TicketDetailDto(
            ticket.Id,
            ticket.Subject,
            ticket.CustomerName,
            ticket.CustomerEmail,
            ticket.Status.ToString(),
            ticket.Priority.ToString(),
            ticket.AssignedAgentId,
            ticket.CreatedAt,
            ticket.Messages.Select(m => new TicketMessageDto(m.Id, m.Sender, m.IsFromAgent, m.Body, m.SentAt)).ToList()
        );
    }
}
