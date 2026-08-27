using Seadora.Finance.Domain.Enums;

namespace Seadora.Finance.Domain.Entities;

// ponytail: plain record of money received - bank-feed auto-match and reconciliation land in a
// later task, ReconciledUtc is the hook they will set.
public class Payment
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public Guid BranchId { get; set; }
    public Guid? CustomerId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = FinanceConstants.ReportingCurrency;
    public PaymentMethod Method { get; set; }
    public string? Reference { get; set; }
    public DateTime ReceivedUtc { get; set; }
    public DateTime? ReconciledUtc { get; set; }
    public string? CreatedBy { get; set; }
}
