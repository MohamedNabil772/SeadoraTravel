namespace Seadora.Finance.Domain.Entities;

public class Payment
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public Guid BranchId { get; set; }
    public Guid? CustomerId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = FinanceConstants.ReportingCurrency;
    public decimal ExchangeRate { get; set; } = 1.0m;
    public decimal SettledAmount { get; set; }
    public string Method { get; set; } = "Card";
    public string? Reference { get; set; }
    public DateTime ReceivedUtc { get; set; }
    public DateTime? ReconciledUtc { get; set; }
    public string? CreatedBy { get; set; }
}
