using System;
using System.Collections.Generic;
using Seadora.Support.Domain.Enums;

namespace Seadora.Support.Domain.Entities;

public class Ticket
{
    public Guid Id { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public Guid? CustomerId { get; set; }
    public Guid BranchId { get; set; }
    public Guid? BookingId { get; set; }
    public string Category { get; set; } = string.Empty;
    public TicketChannel Channel { get; set; }
    public TicketStatus Status { get; set; } = TicketStatus.Open;
    public TicketPriority Priority { get; set; } = TicketPriority.Medium;
    public Guid? AssignedAgentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<TicketMessage> Messages { get; set; } = new();
}
