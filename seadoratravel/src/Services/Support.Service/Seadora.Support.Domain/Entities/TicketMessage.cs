using System;

namespace Seadora.Support.Domain.Entities;

public class TicketMessage
{
    public Guid Id { get; set; }
    public Guid TicketId { get; set; }
    public Ticket Ticket { get; set; } = null!;
    public string Sender { get; set; } = string.Empty;
    public bool IsFromAgent { get; set; }
    public string Body { get; set; } = string.Empty;
    public string? MessageId { get; set; } // For email threading
    public DateTime SentAt { get; set; }
}
