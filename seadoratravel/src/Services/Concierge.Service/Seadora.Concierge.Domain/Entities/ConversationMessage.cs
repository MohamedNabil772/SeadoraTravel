using Seadora.Concierge.Domain.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Seadora.Concierge.Domain.Entities;

public class ConversationMessage
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    
    // User, Assistant, System
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public ConciergeIntent? Intent { get; set; }
    public List<Guid>? SuggestedTourIds { get; set; }
    public DateTime CreatedUtc { get; set; }

    public ConversationSession Session { get; set; } = null!;
}
