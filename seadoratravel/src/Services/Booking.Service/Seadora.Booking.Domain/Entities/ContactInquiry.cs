using System;

namespace Seadora.Booking.Domain.Entities;

public class ContactInquiry
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Interest { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = "New"; // New | Replied
    public string? ReplyMessage { get; set; }
    public DateTime? RepliedAt { get; set; }
}
