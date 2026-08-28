using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Contracts.Support;
using Seadora.Common.Messaging.Outbox;
using Seadora.Support.Application.Interfaces;
using Seadora.Support.Domain.Enums;

namespace Seadora.Support.Application.Commands;

public record UpdateTicketStatusCommand(Guid TicketId, TicketStatus Status) : IRequest;

public class UpdateTicketStatusCommandHandler : IRequestHandler<UpdateTicketStatusCommand>
{
    private readonly ISupportDbContext _context;
    private readonly IOutboxWriter _outbox;

    public UpdateTicketStatusCommandHandler(ISupportDbContext context, IOutboxWriter outbox)
    {
        _context = context;
        _outbox = outbox;
    }

    public async Task Handle(UpdateTicketStatusCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == request.TicketId);
        if (ticket == null) throw new Exception("Ticket not found");

        var oldStatus = ticket.Status;
        ticket.Status = request.Status;
        ticket.UpdatedAt = DateTime.UtcNow;

        _outbox.Enqueue(new TicketStatusChanged(ticket.Id, (int)oldStatus, (int)request.Status));

        await _context.SaveChangesAsync(cancellationToken);
    }
}
