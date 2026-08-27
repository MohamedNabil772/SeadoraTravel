using Microsoft.EntityFrameworkCore;
using Seadora.Finance.Application.Reports;
using Seadora.Finance.Domain.Entities;
using Seadora.Finance.Domain.Enums;
using Seadora.Finance.Domain.Posting;
using Seadora.Finance.Infrastructure.Persistence;

namespace Seadora.UnitTests;

public class FinanceReportTests
{
    private static FinanceDbContext NewDb() =>
        new(new DbContextOptionsBuilder<FinanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    private static BookingFinancialSnapshot Snap(Guid branch, decimal net, decimal tax, decimal supplierCost,
        decimal due, string status = "Recognized", string? tourType = "GROUP", DateTime? date = null, string currency = "USD")
        => new()
        {
            Id = Guid.NewGuid(),
            BookingId = Guid.NewGuid(),
            BranchId = branch,
            TourId = Guid.NewGuid(),
            TourTypeCode = tourType,
            Gross = net,
            Discount = 0m,
            Tax = tax,
            Net = net,
            SupplierCost = supplierCost,
            Margin = net - supplierCost,
            Paid = (net + tax) - due,
            Due = due,
            Currency = currency,
            Status = status,
            BookingDateUtc = date ?? DateTime.UtcNow,
            UpdatedUtc = DateTime.UtcNow
        };

    [Fact]
    public async Task TrialBalance_Is_Balanced_Across_Accounts()
    {
        var db = NewDb();
        var branch = Guid.NewGuid();
        var facts = new RevenueFacts(Guid.NewGuid(), branch, null, Guid.NewGuid(), "GROUP",
            200m, 50m, 30m, 22m, 242m, "USD", Guid.NewGuid(), 40m, DateTime.UtcNow, "evt-1");
        db.JournalEntries.Add(LedgerPosting.RevenueRecognition(facts, 1m));
        db.JournalEntries.Add(LedgerPosting.SupplierAccrual(facts, 1m)!);
        await db.SaveChangesAsync();

        var rows = await new TrialBalanceQueryHandler(db).Handle(new TrialBalanceQuery(new ReportFilter()), default);

        Assert.Equal(rows.Sum(r => r.TotalDebit), rows.Sum(r => r.TotalCredit));
        Assert.True(rows.Count > 0);
    }

    [Fact]
    public async Task ProfitAndLoss_Computes_Net_Profit()
    {
        var db = NewDb();
        var branch = Guid.NewGuid();
        db.BookingFinancialSnapshots.Add(Snap(branch, net: 200m, tax: 20m, supplierCost: 80m, due: 0m));
        db.BookingFinancialSnapshots.Add(Snap(branch, net: 100m, tax: 10m, supplierCost: 40m, due: 0m));
        db.RevenueDaily.Add(new RevenueDaily { Id = Guid.NewGuid(), BranchId = branch, Day = DateTime.UtcNow.Date, Currency = "USD", Refunds = 30m });
        await db.SaveChangesAsync();

        var pl = await new ProfitAndLossQueryHandler(db).Handle(new ProfitAndLossQuery(new ReportFilter()), default);

        Assert.Equal(300m, pl.Net);
        Assert.Equal(120m, pl.SupplierCost);
        Assert.Equal(30m, pl.Refunds);
        Assert.Equal(150m, pl.NetProfit); // 300 - 120 - 30
    }

    [Fact]
    public async Task ArAging_Buckets_By_Age()
    {
        var db = NewDb();
        var branch = Guid.NewGuid();
        var asOf = new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc);
        db.BookingFinancialSnapshots.Add(Snap(branch, 100m, 0m, 0m, due: 100m, date: asOf.AddDays(-10)));  // 0-30
        db.BookingFinancialSnapshots.Add(Snap(branch, 100m, 0m, 0m, due: 50m, date: asOf.AddDays(-45)));   // 31-60
        db.BookingFinancialSnapshots.Add(Snap(branch, 100m, 0m, 0m, due: 25m, date: asOf.AddDays(-100)));  // 90+
        db.BookingFinancialSnapshots.Add(Snap(branch, 100m, 0m, 0m, due: 0m, date: asOf.AddDays(-5)));     // excluded (paid)
        await db.SaveChangesAsync();

        var aging = await new ArAgingQueryHandler(db).Handle(new ArAgingQuery(new ReportFilter(), asOf), default);

        Assert.Equal(100m, aging.Bucket0_30);
        Assert.Equal(50m, aging.Bucket31_60);
        Assert.Equal(25m, aging.Bucket90Plus);
        Assert.Equal(175m, aging.Total);
        Assert.Equal(3, aging.Items.Count);
    }

    [Fact]
    public async Task Revenue_Report_Sums_Recognized_And_Collected()
    {
        var db = NewDb();
        var branch = Guid.NewGuid();
        db.RevenueDaily.Add(new RevenueDaily { Id = Guid.NewGuid(), BranchId = branch, Day = new DateTime(2026, 1, 1), Currency = "USD", Recognized = 100m, Collected = 60m });
        db.RevenueDaily.Add(new RevenueDaily { Id = Guid.NewGuid(), BranchId = branch, Day = new DateTime(2026, 1, 2), Currency = "USD", Recognized = 200m, Collected = 150m });
        await db.SaveChangesAsync();

        var rep = await new RevenueReportQueryHandler(db).Handle(new RevenueReportQuery(new ReportFilter()), default);

        Assert.Equal(300m, rep.TotalRecognized);
        Assert.Equal(210m, rep.TotalCollected);
        Assert.Equal(2, rep.Series.Count);
    }

    [Fact]
    public async Task Tax_Report_Aggregates_By_Branch()
    {
        var db = NewDb();
        var branch = Guid.NewGuid();
        db.BookingFinancialSnapshots.Add(Snap(branch, 100m, tax: 15m, supplierCost: 0m, due: 0m));
        db.BookingFinancialSnapshots.Add(Snap(branch, 100m, tax: 5m, supplierCost: 0m, due: 0m));
        db.BookingFinancialSnapshots.Add(Snap(branch, 100m, tax: 99m, supplierCost: 0m, due: 0m, status: "Cancelled"));
        await db.SaveChangesAsync();

        var tax = await new TaxReportQueryHandler(db).Handle(new TaxReportQuery(new ReportFilter()), default);

        Assert.Equal(20m, tax.Total); // cancelled excluded
    }

    [Fact]
    public async Task GeneralLedger_Filters_By_Branch_And_Pages()
    {
        var db = NewDb();
        var branchA = Guid.NewGuid();
        var branchB = Guid.NewGuid();
        var fa = new RevenueFacts(Guid.NewGuid(), branchA, null, Guid.NewGuid(), "GROUP", 100m, 0m, 0m, 0m, 100m, "USD", null, 0m, DateTime.UtcNow, "a");
        var fb = new RevenueFacts(Guid.NewGuid(), branchB, null, Guid.NewGuid(), "GROUP", 100m, 0m, 0m, 0m, 100m, "USD", null, 0m, DateTime.UtcNow, "b");
        db.JournalEntries.Add(LedgerPosting.RevenueRecognition(fa, 1m));
        db.JournalEntries.Add(LedgerPosting.RevenueRecognition(fb, 1m));
        await db.SaveChangesAsync();

        var res = await new GeneralLedgerQueryHandler(db).Handle(
            new GeneralLedgerQuery(new ReportFilter(BranchId: branchA)), default);

        Assert.All(res.Items, l => Assert.Equal(branchA, l.BranchId));
        Assert.Equal(2, res.Total); // AR + Revenue lines for the single branchA entry
    }

    [Fact]
    public async Task SupplierPayables_Computes_Due()
    {
        var db = NewDb();
        db.SupplierSettlements.Add(new SupplierSettlement
        {
            Id = Guid.NewGuid(),
            SupplierId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            PeriodStart = new DateTime(2026, 1, 1),
            PeriodEnd = new DateTime(2026, 2, 1),
            AccruedAmount = 500m,
            PaidAmount = 200m,
            Currency = "USD",
            Status = SettlementStatus.PartiallyPaid
        });
        await db.SaveChangesAsync();

        var rows = await new SupplierPayablesQueryHandler(db).Handle(new SupplierPayablesQuery(new ReportFilter()), default);

        Assert.Single(rows);
        Assert.Equal(300m, rows[0].Due);
    }
}
