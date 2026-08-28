using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Seadora.Contracts.Support;
using Seadora.Common.Messaging.Outbox;
using Seadora.Support.Application.Interfaces;
using Seadora.Support.Domain.Entities;
using Seadora.Support.Domain.Enums;

namespace Seadora.Support.Application.Commands;

public record CreateTicketCommand(string Subject, string CustomerName, string CustomerEmail, Guid? CustomerId, TicketChannel Channel, TicketPriority Priority, string MessageBody) : IRequest<Guid>;

public class CreateTicketCommandHandler : IRequestHandler<CreateTicketCommand, Guid>
{
    private readonly ISupportDbContext _context;
    private readonly IOutboxWriter _outbox;

    public CreateTicketCommandHandler(ISupportDbContext context, IOutboxWriter outbox)
    {
        _context = context;
        _outbox = outbox;
    }

    public async Task<Guid> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = new Ticket
        {
            Id = Guid.NewGuid(),
            Subject = request.Subject,
            CustomerName = request.CustomerName,
            CustomerEmail = request.CustomerEmail,
            CustomerId = request.CustomerId,
            Channel = request.Channel,
            Priority = request.Priority,
            Status = TicketStatus.Open,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var message = new TicketMessage
        {
            Id = Guid.NewGuid(),
            TicketId = ticket.Id,
            Sender = request.CustomerName,
            IsFromAgent = false,
            Body = request.MessageBody,
            SentAt = DateTime.UtcNow
        };

        ticket.Messages.Add(message);
        _context.Tickets.Add(ticket);

        _outbox.Enqueue(new TicketCreated(ticket.Id, ticket.CustomerEmail, ticket.Subject));

        await _context.SaveChangesAsync(cancellationToken);

        return ticket.Id;
    }
}
