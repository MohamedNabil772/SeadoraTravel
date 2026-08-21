using Seadora.Booking.Domain.Enums;

namespace Seadora.Booking.Application.DTOs;

public record NotificationDto(
    Guid Id,
    string Title,
    string Message,
    NotificationType Type,
    string? ReferenceId,
    string? MetadataJson,
    bool IsRead,
    DateTime CreatedAt,
    DateTime? ReadAt
);
