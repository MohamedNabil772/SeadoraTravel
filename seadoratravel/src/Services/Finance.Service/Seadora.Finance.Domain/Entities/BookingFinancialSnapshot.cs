namespace Seadora.Finance.Domain.Entities;

// ponytail: pre-aggregated per booking so dashboards never join the ledger. Populated by the
// projection task; AR-aging and dashboard KPIs stay query-time derivations over this table.
public class BookingFinancialSnapshot
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public Guid BranchId { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid TourId { get; set; }
    public string? TourTypeCode { get; set; }
    public decimal Gross { get; set; }
    public decimal Discount { get; set; }
    public decimal Tax { get; set; }
    public decimal Net { get; set; }
    public decimal SupplierCost { get; set; }
    public decimal Margin { get; set; }
    public decimal Paid { get; set; }
    public decimal Due { get; set; }
    public string Currency { get; set; } = FinanceConstants.ReportingCurrency;
    public string Status { get; set; } = string.Empty;
    public DateTime BookingDateUtc { get; set; }
    public DateTime UpdatedUtc { get; set; }
}
