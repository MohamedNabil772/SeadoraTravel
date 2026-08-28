using MediatR;
using System;

namespace Seadora.Concierge.Application.Commands;

public class HandoffToHumanCommand : IRequest<HandoffToHumanResponse>
{
    public Guid SessionId { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class HandoffToHumanResponse
{
    public string TicketReference { get; set; } = string.Empty;
}
