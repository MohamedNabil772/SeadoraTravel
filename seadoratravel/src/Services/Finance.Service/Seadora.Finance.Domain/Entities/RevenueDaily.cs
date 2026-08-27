namespace Seadora.Finance.Domain.Entities;

public class RevenueDaily
{
    public Guid Id { get; set; }
    public Guid BranchId { get; set; }
    public DateTime Day { get; set; }
    public decimal Recognized { get; set; }
    public decimal Collected { get; set; }
    public decimal Refunds { get; set; }
    public decimal SupplierCost { get; set; }
    public decimal Margin { get; set; }
    public string Currency { get; set; } = FinanceConstants.ReportingCurrency;
}
