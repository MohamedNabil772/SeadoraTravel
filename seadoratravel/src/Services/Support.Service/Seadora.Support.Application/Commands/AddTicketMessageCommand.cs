using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Contracts.Support;
using Seadora.Common.Messaging.Outbox;
using Seadora.Support.Application.Interfaces;
using Seadora.Support.Domain.Entities;

namespace Seadora.Support.Application.Commands;

public record AddTicketMessageCommand(Guid TicketId, string Sender, bool IsFromAgent, string Body, string? MessageId) : IRequest;

public class AddTicketMessageCommandHandler : IRequestHandler<AddTicketMessageCommand>
{
    private readonly ISupportDbContext _context;
    private readonly IOutboxWriter _outbox;

    public AddTicketMessageCommandHandler(ISupportDbContext context, IOutboxWriter outbox)
    {
        _context = context;
        _outbox = outbox;
    }

    public async Task Handle(AddTicketMessageCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == request.TicketId);
        if (ticket == null) throw new Exception("Ticket not found");

        var message = new TicketMessage
        {
            Id = Guid.NewGuid(),
            TicketId = request.TicketId,
            Sender = request.Sender,
            IsFromAgent = request.IsFromAgent,
            Body = request.Body,
            MessageId = request.MessageId,
            SentAt = DateTime.UtcNow
        };

        ticket.UpdatedAt = DateTime.UtcNow;
        _context.TicketMessages.Add(message);

        _outbox.Enqueue(new TicketReplied(ticket.Id, message.Id, message.Body, message.IsFromAgent));

        await _context.SaveChangesAsync(cancellationToken);
    }
}
