namespace Seadora.Customer.Application.DTOs;

public record CustomerDto(
    Guid Id,
    string FullName,
    string Email,
    string? Phone,
    string? Nationality,
    bool MarketingConsent,
    DateTime CreatedUtc);

public record CustomerDocumentDto(
    Guid Id,
    Guid CustomerId,
    string DocumentType,
    string FileRef,
    string FileName,
    DateTime UploadedUtc,
    DateTime? RetentionUntilUtc);

public record CustomerBookingHistoryDto(
    Guid Id,
    Guid BookingId,
    Guid TourId,
    DateTime? TourDate,
    decimal Amount,
    string Currency,
    DateTime PlacedUtc);

public record CustomerDetailDto(
    Guid Id,
    string FullName,
    string Email,
    string? Phone,
    string? Nationality,
    string? PassportNumber,
    string? Notes,
    bool MarketingConsent,
    DateTime? ConsentUpdatedUtc,
    DateTime CreatedUtc,
    DateTime UpdatedUtc,
    List<CustomerDocumentDto> Documents,
    List<CustomerBookingHistoryDto> BookingHistory);
