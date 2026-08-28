using System;

namespace Seadora.Concierge.Application.IntegrationEvents;

public class InquiryReceivedIntegrationEvent
{
    public Guid SessionId { get; set; }
    public string? CustomerEmail { get; set; }
    public string Transcript { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string TicketReference { get; set; } = string.Empty;
    public DateTime OccurredOn { get; set; }
}
