using MassTransit;
using MediatR;
using Seadora.Concierge.Domain.Entities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Seadora.Concierge.Application.IntegrationEvents;

namespace Seadora.Concierge.Application.Commands;

public class HandoffToHumanCommandHandler : IRequestHandler<HandoffToHumanCommand, HandoffToHumanResponse>
{
    private readonly IConciergeDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;

    public HandoffToHumanCommandHandler(IConciergeDbContext dbContext, IPublishEndpoint publishEndpoint)
    {
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<HandoffToHumanResponse> Handle(HandoffToHumanCommand request, CancellationToken cancellationToken)
    {
        var session = await _dbContext.ConversationSessions
            .Include(s => s.Messages)
            .FirstOrDefaultAsync(s => s.Id == request.SessionId, cancellationToken);

        if (session == null)
        {
            throw new Exception("Session not found");
        }

        var transcript = string.Join("\n", session.Messages.OrderBy(m => m.CreatedUtc).Select(m => $"{m.Role}: {m.Content}"));
        var ticketRef = $"TKT-{DateTime.UtcNow.Ticks}";

        // Enqueue InquiryReceived
        await _publishEndpoint.Publish(new InquiryReceivedIntegrationEvent
        {
            SessionId = request.SessionId,
            CustomerEmail = session.CustomerEmail,
            Transcript = transcript,
            Reason = request.Reason,
            TicketReference = ticketRef,
            OccurredOn = DateTime.UtcNow
        }, cancellationToken);

        return new HandoffToHumanResponse
        {
            TicketReference = ticketRef
        };
    }
}
