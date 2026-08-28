using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Support.Application.Interfaces;

namespace Seadora.Support.Application.Commands;

public record AssignTicketCommand(Guid TicketId, Guid AgentId) : IRequest;

public class AssignTicketCommandHandler : IRequestHandler<AssignTicketCommand>
{
    private readonly ISupportDbContext _context;

    public AssignTicketCommandHandler(ISupportDbContext context)
    {
        _context = context;
    }

    public async Task Handle(AssignTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == request.TicketId, cancellationToken);
        if (ticket == null) throw new Exception("Ticket not found");

        ticket.AssignedAgentId = request.AgentId;
        ticket.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
