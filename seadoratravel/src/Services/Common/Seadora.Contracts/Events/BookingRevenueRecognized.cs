using Seadora.Contracts.Messaging;

namespace Seadora.Contracts.Events;

// ponytail: carries the full breakdown so Finance never calls back into Booking to post the accrual.
public record BookingRevenueRecognized : IntegrationEvent
{
    public Guid BookingId { get; init; }
    public Guid BranchId { get; init; }
    public Guid? CustomerId { get; init; }
    public Guid TourId { get; init; }
    public string? TourTypeCode { get; init; }
    public decimal Subtotal { get; init; }
    public decimal AddonsTotal { get; init; }
    public decimal Discount { get; init; }
    public decimal TaxTotal { get; init; }
    public decimal Total { get; init; }
    public string Currency { get; init; } = "EUR";
    public Guid? SupplierId { get; init; }
    public decimal SupplierPercentage { get; init; }
}
