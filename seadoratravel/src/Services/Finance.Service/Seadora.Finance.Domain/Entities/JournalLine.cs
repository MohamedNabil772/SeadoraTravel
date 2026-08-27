namespace Seadora.Finance.Domain.Entities;

public class JournalLine
{
    public Guid Id { get; set; }
    public Guid JournalEntryId { get; set; }
    public Guid AccountId { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public string Currency { get; set; } = FinanceConstants.ReportingCurrency;
    public decimal FxRate { get; set; }
    public decimal ReportingDebit { get; set; }
    public decimal ReportingCredit { get; set; }
}
