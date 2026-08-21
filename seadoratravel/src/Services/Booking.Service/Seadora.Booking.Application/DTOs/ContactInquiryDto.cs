using Seadora.Booking.Domain.Enums;

namespace Seadora.Booking.Application.DTOs;

public record ContactInquiryDto(
    Guid Id,
    string FullName,
    string Email,
    string? Phone,
    string? DestinationInterest,
    string? DateOrGuests,
    string Message,
    InquiryStatus Status,
    string? AdminNotes,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
