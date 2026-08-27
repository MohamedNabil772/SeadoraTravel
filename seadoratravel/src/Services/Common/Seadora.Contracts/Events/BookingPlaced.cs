using Seadora.Contracts.Messaging;

namespace Seadora.Contracts.Events;

public record BookingPlaced : IntegrationEvent
{
    public Guid BookingId { get; init; }
    public Guid BranchId { get; init; }
    public string CustomerEmail { get; init; } = default!;
    public string CustomerName { get; init; } = default!;
    public string? Phone { get; init; }
    public Guid TourId { get; init; }
    public DateTime? TourDate { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "EUR";
}
