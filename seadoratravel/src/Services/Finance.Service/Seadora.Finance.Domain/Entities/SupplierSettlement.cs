using Seadora.Finance.Domain.Enums;

namespace Seadora.Finance.Domain.Entities;

public class SupplierSettlement
{
    public Guid Id { get; set; }
    public Guid SupplierId { get; set; }
    public Guid BranchId { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public decimal AccruedAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public string Currency { get; set; } = FinanceConstants.ReportingCurrency;
    public SettlementStatus Status { get; set; }
}
