using Seadora.Finance.Domain;
using Seadora.Finance.Domain.Entities;

namespace Seadora.UnitTests;

public class FinanceJournalEntryTests
{
    private static readonly Guid BranchA = Guid.Parse("00000000-0000-0000-0000-0000000000aa");

    private static JournalEntry Create(params JournalLineDraft[] lines) =>
        JournalEntry.Create(DateTime.UtcNow, "test entry", BranchA, null, null, lines);

    private static JournalLineDraft Debit(Guid account, decimal amount, string currency = FinanceConstants.ReportingCurrency, decimal fx = 1m)
        => new(account, amount, 0m, currency, fx);

    private static JournalLineDraft Credit(Guid account, decimal amount, string currency = FinanceConstants.ReportingCurrency, decimal fx = 1m)
        => new(account, 0m, amount, currency, fx);

    [Fact]
    public void Create_BalancedTwoLineEntry_Succeeds()
    {
        var entry = Create(
            Debit(ChartOfAccounts.CashBank, 100m),
            Credit(ChartOfAccounts.Revenue, 100m));

        Assert.Equal(2, entry.Lines.Count);
        Assert.Equal(BranchA, entry.BranchId);
        Assert.Equal(100m, entry.Lines.Sum(l => l.ReportingDebit));
        Assert.Equal(100m, entry.Lines.Sum(l => l.ReportingCredit));
        Assert.All(entry.Lines, l => Assert.Equal(entry.Id, l.JournalEntryId));
    }

    [Fact]
    public void Create_Unbalanced_Throws()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => Create(
            Debit(ChartOfAccounts.CashBank, 100m),
            Credit(ChartOfAccounts.Revenue, 90m)));

        Assert.Contains("not balanced", ex.Message);
    }

    [Fact]
    public void Create_LineWithBothSidesPositive_Throws()
    {
        Assert.Throws<ArgumentException>(() => Create(
            new JournalLineDraft(ChartOfAccounts.CashBank, 100m, 100m, "USD", 1m),
            Credit(ChartOfAccounts.Revenue, 100m)));
    }

    [Fact]
    public void Create_LineWithBothSidesZero_Throws()
    {
        Assert.Throws<ArgumentException>(() => Create(
            new JournalLineDraft(ChartOfAccounts.CashBank, 0m, 0m, "USD", 1m),
            Credit(ChartOfAccounts.Revenue, 100m)));
    }

    [Fact]
    public void Create_FewerThanTwoLines_Throws()
    {
        Assert.Throws<ArgumentException>(() => Create(Debit(ChartOfAccounts.CashBank, 100m)));
    }

    [Fact]
    public void Create_ConvertsForeignCurrencyToReportingAmounts()
    {
        var entry = Create(
            Debit(ChartOfAccounts.CashBank, 100m, "EUR", 1.10m),
            Credit(ChartOfAccounts.Revenue, 110m));

        var eurLine = entry.Lines.Single(l => l.Currency == "EUR");
        Assert.Equal(110.00m, eurLine.ReportingDebit);
        Assert.Equal(0m, eurLine.ReportingCredit);
        Assert.Equal(entry.Lines.Sum(l => l.ReportingDebit), entry.Lines.Sum(l => l.ReportingCredit));
    }

    [Fact]
    public void Create_CrossCurrencyEntry_BalancesInReportingCurrency()
    {
        var entry = Create(
            Debit(ChartOfAccounts.AccountsReceivable, 200m, "EUR", 1.10m),
            Credit(ChartOfAccounts.Revenue, 220m, "USD", 1m));

        Assert.Equal(220.00m, entry.Lines.Sum(l => l.ReportingDebit));
        Assert.Equal(220.00m, entry.Lines.Sum(l => l.ReportingCredit));
    }

    [Fact]
    public void Snapshot_NetAndDue_Arithmetic()
    {
        var snapshot = new BookingFinancialSnapshot
        {
            Gross = 1000m,
            Discount = 100m,
            Tax = 50m,
            SupplierCost = 600m,
            Paid = 400m
        };
        snapshot.Net = snapshot.Gross - snapshot.Discount + snapshot.Tax;
        snapshot.Margin = snapshot.Net - snapshot.SupplierCost;
        snapshot.Due = snapshot.Net - snapshot.Paid;

        Assert.Equal(950m, snapshot.Net);
        Assert.Equal(350m, snapshot.Margin);
        Assert.Equal(550m, snapshot.Due);
    }
}
