using Seadora.Contracts.Messaging;

namespace Seadora.Contracts.Events;

public record PaymentRecorded : IntegrationEvent
{
    public Guid PaymentId { get; init; }
    public Guid BookingId { get; init; }
    public Guid BranchId { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "EUR";
    public decimal CumulativePaid { get; init; }
    public decimal BookingTotal { get; init; }
    public string Method { get; init; } = "Cash";
    public DateTime ReceivedUtc { get; init; }
}
