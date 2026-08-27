namespace Seadora.Finance.Domain.Posting;

// ponytail: Domain stays free of Seadora.Contracts - the Application consumers map events to these facts.
public record RevenueFacts(
    Guid BookingId,
    Guid BranchId,
    Guid? CustomerId,
    Guid TourId,
    string? TourTypeCode,
    decimal Subtotal,
    decimal AddonsTotal,
    decimal Discount,
    decimal TaxTotal,
    decimal Total,
    string Currency,
    Guid? SupplierId,
    decimal SupplierPercentage,
    DateTime OccurredUtc,
    string? SourceEventId)
{
    public decimal Gross => Subtotal + AddonsTotal;
}
