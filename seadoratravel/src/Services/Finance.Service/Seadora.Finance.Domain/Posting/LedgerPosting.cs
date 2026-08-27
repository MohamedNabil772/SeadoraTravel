using Seadora.Finance.Domain.Entities;

namespace Seadora.Finance.Domain.Posting;

/// <summary>
/// Pure double-entry posting rules. Each method returns a balanced <see cref="JournalEntry"/>
/// (or null when there is nothing to post). No I/O - the caller resolves FX and persists.
/// </summary>
public static class LedgerPosting
{
    private static decimal R(decimal v) => Math.Round(v, 2, MidpointRounding.AwayFromZero);

    public static decimal SupplierCostOf(RevenueFacts f) =>
        f.SupplierId is null || f.SupplierPercentage <= 0
            ? 0m
            : R(f.Gross * f.SupplierPercentage / 100m);

    /// <summary>Dr AR (Total), Cr Revenue (Gross), Dr Discounts, Cr TaxPayable. Balances by the Money identity.</summary>
    public static JournalEntry RevenueRecognition(RevenueFacts f, decimal fxRate)
    {
        var lines = new List<JournalLineDraft>
        {
            new(ChartOfAccounts.AccountsReceivable, R(f.Total), 0m, f.Currency, fxRate),
            new(ChartOfAccounts.Revenue, 0m, R(f.Gross), f.Currency, fxRate)
        };
        if (f.Discount > 0)
            lines.Add(new(ChartOfAccounts.Discounts, R(f.Discount), 0m, f.Currency, fxRate));
        if (f.TaxTotal > 0)
            lines.Add(new(ChartOfAccounts.TaxPayable, 0m, R(f.TaxTotal), f.Currency, fxRate));

        return JournalEntry.Create(f.OccurredUtc, $"Revenue recognition for booking {f.BookingId}",
            f.BranchId, f.BookingId, f.SourceEventId, lines);
    }

    /// <summary>Dr SupplierCostExpense / Cr SupplierPayable. Null when there is no supplier cost.</summary>
    public static JournalEntry? SupplierAccrual(RevenueFacts f, decimal fxRate)
    {
        var cost = SupplierCostOf(f);
        if (cost <= 0) return null;
        var lines = new List<JournalLineDraft>
        {
            new(ChartOfAccounts.SupplierCostExpense, cost, 0m, f.Currency, fxRate),
            new(ChartOfAccounts.SupplierPayable, 0m, cost, f.Currency, fxRate)
        };
        return JournalEntry.Create(f.OccurredUtc, $"Supplier cost accrual for booking {f.BookingId}",
            f.BranchId, f.BookingId, f.SourceEventId + ":supplier", lines);
    }

    /// <summary>Reverses a recognition from the persisted snapshot (used on cancellation).</summary>
    public static JournalEntry RevenueReversal(BookingFinancialSnapshot snap, DateTime occurredUtc,
        string currency, decimal fxRate, string? sourceEventId)
    {
        // snap.Net = Gross - Discount; Total = Gross - Discount + Tax = Net + Tax.
        var total = R(snap.Net + snap.Tax);
        var lines = new List<JournalLineDraft>
        {
            new(ChartOfAccounts.AccountsReceivable, 0m, total, currency, fxRate),
            new(ChartOfAccounts.Revenue, R(snap.Gross), 0m, currency, fxRate)
        };
        if (snap.Discount > 0)
            lines.Add(new(ChartOfAccounts.Discounts, 0m, R(snap.Discount), currency, fxRate));
        if (snap.Tax > 0)
            lines.Add(new(ChartOfAccounts.TaxPayable, R(snap.Tax), 0m, currency, fxRate));

        return JournalEntry.Create(occurredUtc, $"Reversal on cancellation for booking {snap.BookingId}",
            snap.BranchId, snap.BookingId, sourceEventId, lines);
    }

    /// <summary>Dr SupplierPayable / Cr SupplierCostExpense - reverses an accrual on cancellation.</summary>
    public static JournalEntry? SupplierReversal(BookingFinancialSnapshot snap, DateTime occurredUtc,
        string currency, decimal fxRate, string? sourceEventId)
    {
        if (snap.SupplierCost <= 0) return null;
        var cost = R(snap.SupplierCost);
        var lines = new List<JournalLineDraft>
        {
            new(ChartOfAccounts.SupplierPayable, cost, 0m, currency, fxRate),
            new(ChartOfAccounts.SupplierCostExpense, 0m, cost, currency, fxRate)
        };
        return JournalEntry.Create(occurredUtc, $"Supplier accrual reversal for booking {snap.BookingId}",
            snap.BranchId, snap.BookingId, sourceEventId + ":supplier", lines);
    }

    /// <summary>Dr Refunds / Cr Cash.</summary>
    public static JournalEntry Refund(Guid bookingId, Guid branchId, decimal amount, string currency,
        decimal fxRate, DateTime occurredUtc, string? sourceEventId)
    {
        var amt = R(amount);
        var lines = new List<JournalLineDraft>
        {
            new(ChartOfAccounts.Refunds, amt, 0m, currency, fxRate),
            new(ChartOfAccounts.CashBank, 0m, amt, currency, fxRate)
        };
        return JournalEntry.Create(occurredUtc, $"Refund for booking {bookingId}",
            branchId, bookingId, sourceEventId, lines);
    }

    /// <summary>Dr Cash / Cr AR - a customer receipt reduces receivable.</summary>
    public static JournalEntry PaymentReceipt(Guid bookingId, Guid branchId, decimal amount, string currency,
        decimal fxRate, DateTime occurredUtc, string? sourceEventId)
    {
        var amt = R(amount);
        var lines = new List<JournalLineDraft>
        {
            new(ChartOfAccounts.CashBank, amt, 0m, currency, fxRate),
            new(ChartOfAccounts.AccountsReceivable, 0m, amt, currency, fxRate)
        };
        return JournalEntry.Create(occurredUtc, $"Payment receipt for booking {bookingId}",
            branchId, bookingId, sourceEventId, lines);
    }
}
