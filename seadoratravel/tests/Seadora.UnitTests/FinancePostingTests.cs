using Seadora.Finance.Domain;
using Seadora.Finance.Domain.Entities;
using Seadora.Finance.Domain.Posting;

namespace Seadora.UnitTests;

public class FinancePostingTests
{
    private static RevenueFacts Facts(decimal subtotal = 100m, decimal addons = 0m, decimal discount = 0m,
        decimal tax = 0m, string currency = "USD", Guid? supplierId = null, decimal supplierPct = 0m)
    {
        var total = subtotal + addons - discount + tax;
        return new RevenueFacts(Guid.NewGuid(), Guid.NewGuid(), null, Guid.NewGuid(), "GROUP",
            subtotal, addons, discount, tax, total, currency, supplierId, supplierPct, DateTime.UtcNow, Guid.NewGuid().ToString());
    }

    private static (decimal Debit, decimal Credit) Totals(JournalEntry e) =>
        (e.Lines.Sum(l => l.ReportingDebit), e.Lines.Sum(l => l.ReportingCredit));

    [Fact]
    public void RevenueRecognition_NoDiscountNoTax_Balances_TwoLines()
    {
        var e = LedgerPosting.RevenueRecognition(Facts(subtotal: 100m), 1m);
        Assert.Equal(2, e.Lines.Count);
        var (d, c) = Totals(e);
        Assert.Equal(d, c);
        Assert.Equal(100m, e.Lines.Single(l => l.AccountId == ChartOfAccounts.AccountsReceivable).Debit);
        Assert.Equal(100m, e.Lines.Single(l => l.AccountId == ChartOfAccounts.Revenue).Credit);
    }

    [Fact]
    public void RevenueRecognition_WithDiscount_Balances()
    {
        var e = LedgerPosting.RevenueRecognition(Facts(subtotal: 100m, discount: 20m), 1m);
        var (d, c) = Totals(e);
        Assert.Equal(d, c);
        Assert.Equal(20m, e.Lines.Single(l => l.AccountId == ChartOfAccounts.Discounts).Debit);
    }

    [Fact]
    public void RevenueRecognition_WithTax_Balances()
    {
        var e = LedgerPosting.RevenueRecognition(Facts(subtotal: 100m, tax: 14m), 1m);
        var (d, c) = Totals(e);
        Assert.Equal(d, c);
        Assert.Equal(14m, e.Lines.Single(l => l.AccountId == ChartOfAccounts.TaxPayable).Credit);
    }

    [Fact]
    public void RevenueRecognition_DiscountAndTax_Balances()
    {
        var e = LedgerPosting.RevenueRecognition(Facts(subtotal: 200m, addons: 50m, discount: 30m, tax: 22m), 1m);
        var (d, c) = Totals(e);
        Assert.Equal(d, c);
    }

    [Fact]
    public void SupplierAccrual_ComputesCostAndBalances()
    {
        var f = Facts(subtotal: 100m, addons: 100m, supplierId: Guid.NewGuid(), supplierPct: 20m);
        Assert.Equal(40m, LedgerPosting.SupplierCostOf(f)); // 200 * 20%
        var e = LedgerPosting.SupplierAccrual(f, 1m)!;
        var (d, c) = Totals(e);
        Assert.Equal(d, c);
        Assert.Equal(40m, e.Lines.Single(l => l.AccountId == ChartOfAccounts.SupplierCostExpense).Debit);
    }

    [Fact]
    public void SupplierAccrual_NoSupplier_ReturnsNull()
    {
        Assert.Null(LedgerPosting.SupplierAccrual(Facts(), 1m));
        Assert.Equal(0m, LedgerPosting.SupplierCostOf(Facts()));
    }

    [Fact]
    public void Refund_Balances()
    {
        var e = LedgerPosting.Refund(Guid.NewGuid(), Guid.NewGuid(), 75m, "USD", 1m, DateTime.UtcNow, "src");
        var (d, c) = Totals(e);
        Assert.Equal(d, c);
        Assert.Equal(75m, e.Lines.Single(l => l.AccountId == ChartOfAccounts.Refunds).Debit);
    }

    [Fact]
    public void ForeignCurrency_ReportingAmountsUseRate()
    {
        var e = LedgerPosting.RevenueRecognition(Facts(subtotal: 100m, currency: "EUR"), 1.10m);
        Assert.Equal(110m, e.Lines.Single(l => l.AccountId == ChartOfAccounts.AccountsReceivable).ReportingDebit);
        var (d, c) = Totals(e);
        Assert.Equal(d, c);
    }

    [Fact]
    public void Recognition_Then_Reversal_NetsToZero()
    {
        var f = Facts(subtotal: 200m, addons: 50m, discount: 30m, tax: 22m);
        var recog = LedgerPosting.RevenueRecognition(f, 1m);
        var snap = new BookingFinancialSnapshot
        {
            BookingId = f.BookingId, BranchId = f.BranchId,
            Gross = f.Gross, Discount = f.Discount, Tax = f.TaxTotal,
            Net = f.Gross - f.Discount, SupplierCost = 0m
        };
        var rev = LedgerPosting.RevenueReversal(snap, DateTime.UtcNow, "USD", 1m, "src");

        // per account, recognition debit-credit + reversal debit-credit == 0
        var all = recog.Lines.Concat(rev.Lines).GroupBy(l => l.AccountId);
        foreach (var g in all)
            Assert.Equal(0m, g.Sum(l => l.ReportingDebit - l.ReportingCredit));
    }
}
