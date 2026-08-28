using System;
using System.Collections.Generic;

namespace Seadora.Concierge.Domain.Entities;

public class ConversationSession
{
    public Guid Id { get; set; }
    public Guid BranchId { get; set; }
    public Guid? VisitorId { get; set; }
    public string? CustomerEmail { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime LastActiveUtc { get; set; }

    public List<ConversationMessage> Messages { get; set; } = new();
}
