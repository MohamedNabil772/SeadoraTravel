using System;
using System.Text.Json;
using Seadora.Booking.Domain.Enums;

namespace Seadora.Booking.Domain.Entities;

public class Notification
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;
    public NotificationType Type { get; private set; }
    public string? ReferenceId { get; private set; }
    public string? MetadataJson { get; private set; }
    public bool IsRead { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ReadAt { get; private set; }

    private Notification() { } // EF Core

    public static Notification CreateBookingNotification(Guid bookingId, string reference, string guestName, string tourName, decimal totalAmount)
    {
        var metadata = new { BookingId = bookingId, Reference = reference, GuestName = guestName, TourName = tourName, TotalAmount = totalAmount };
        return new Notification
        {
            Id = Guid.NewGuid(),
            Title = $"New Booking: {reference}",
            Message = $"{guestName} booked '{tourName}' for ${totalAmount}.",
            Type = NotificationType.BookingCreated,
            ReferenceId = bookingId.ToString(),
            MetadataJson = JsonSerializer.Serialize(metadata),
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static Notification CreateInquiryNotification(Guid inquiryId, string guestName, string destination, string email)
    {
        var metadata = new { InquiryId = inquiryId, GuestName = guestName, Destination = destination, Email = email };
        return new Notification
        {
            Id = Guid.NewGuid(),
            Title = $"New Inquiry from {guestName}",
            Message = $"Inquiry for {destination}. Contact: {email}",
            Type = NotificationType.ContactInquiry,
            ReferenceId = inquiryId.ToString(),
            MetadataJson = JsonSerializer.Serialize(metadata),
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static Notification Create(NotificationType type, string title, string message, string? referenceId = null)
    {
        return new Notification
        {
            Id = Guid.NewGuid(),
            Type = type,
            Title = title,
            Message = message,
            ReferenceId = referenceId,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void MarkAsRead()
    {
        if (IsRead) return;
        IsRead = true;
        ReadAt = DateTime.UtcNow;
    }
}
