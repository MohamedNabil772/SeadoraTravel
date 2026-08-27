namespace Seadora.Customer.Domain.Entities;

// ponytail: records booking *placement* only. Live status (confirmed/cancelled) needs a separate
// BookingStatusChanged event + a status column - add it when the CRM actually shows status.
public class CustomerBookingHistory
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid BookingId { get; set; }
    public Guid BranchId { get; set; }
    public Guid TourId { get; set; }
    public DateTime? TourDate { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "EUR";
    public DateTime PlacedUtc { get; set; }
}
