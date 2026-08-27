using Seadora.Contracts.Messaging;

namespace Seadora.Contracts.Events;

public record RefundIssued : IntegrationEvent
{
    public Guid BookingId { get; init; }
    public Guid BranchId { get; init; }
    public Guid? PaymentId { get; init; }
    public decimal RefundAmount { get; init; }
    public string Currency { get; init; } = "EUR";
    public string? Reason { get; init; }
}
