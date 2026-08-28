using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Seadora.Support.Domain.Enums;
using Seadora.Support.Domain.Entities;
using Seadora.Support.Application.Interfaces;

namespace Seadora.Support.Application.Commands;

public record CreateCustomerTicketCommand(Guid CustomerId, Guid BranchId, string Subject, string Description, Guid? BookingId, string Category) : IRequest<Guid>;

public class CreateCustomerTicketCommandHandler : IRequestHandler<CreateCustomerTicketCommand, Guid>
{
    private readonly ISupportDbContext _context;

    public CreateCustomerTicketCommandHandler(ISupportDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateCustomerTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = new Ticket
        {
            Id = Guid.NewGuid(),
            CustomerId = request.CustomerId,
            BranchId = request.BranchId,
            Subject = request.Subject,
            BookingId = request.BookingId,
            Category = request.Category,
            Channel = TicketChannel.Web,
            Status = TicketStatus.Open,
            Priority = TicketPriority.Medium,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        ticket.Messages.Add(new TicketMessage
        {
            Id = Guid.NewGuid(),
            TicketId = ticket.Id,
            Sender = request.CustomerId.ToString(),
            IsFromAgent = false,
            Body = request.Description,
            SentAt = DateTime.UtcNow
        });

        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync(cancellationToken);
        return ticket.Id;
    }
}
