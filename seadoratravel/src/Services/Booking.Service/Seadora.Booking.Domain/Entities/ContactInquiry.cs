using System;
using Seadora.Booking.Domain.Enums;

namespace Seadora.Booking.Domain.Entities;

public class ContactInquiry
{
    public Guid Id { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? Phone { get; private set; }
    public string? DestinationInterest { get; private set; }
    public string? DateOrGuests { get; private set; }
    public string Message { get; private set; } = string.Empty;
    public InquiryStatus Status { get; private set; }
    public string? AdminNotes { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private ContactInquiry() { } // EF Core

    public ContactInquiry(string fullName, string email, string? phone, string? destinationInterest, string? dateOrGuests, string message)
    {
        Id = Guid.NewGuid();
        FullName = fullName;
        Email = email;
        Phone = phone;
        DestinationInterest = destinationInterest;
        DateOrGuests = dateOrGuests;
        Message = message;
        Status = InquiryStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(InquiryStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateAdminNotes(string notes)
    {
        AdminNotes = notes;
        UpdatedAt = DateTime.UtcNow;
    }
}
