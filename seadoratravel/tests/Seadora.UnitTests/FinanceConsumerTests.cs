using Microsoft.EntityFrameworkCore;
using Seadora.Common.Messaging.Idempotency;
using Seadora.Contracts.Events;
using Seadora.Finance.Application.Integration;
using Seadora.Finance.Domain;
using Seadora.Finance.Domain.Entities;
using Seadora.Finance.Infrastructure.Persistence;

namespace Seadora.UnitTests;

public class FinanceConsumerTests
{
    private static (FinanceEventConsumers Consumer, FinanceDbContext Db) Build()
    {
        var options = new DbContextOptionsBuilder<FinanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var db = new FinanceDbContext(options);
        var consumer = new FinanceEventConsumers(db, new IdempotentConsumer(db));
        return (consumer, db);
    }

    private static BookingRevenueRecognized Revenue(Guid bookingId, decimal subtotal = 100m, decimal addons = 0m,
        decimal discount = 0m, decimal tax = 0m, string currency = "USD", Guid? supplierId = null, decimal supplierPct = 0m)
    {
        var total = subtotal + addons - discount + tax;
        return new BookingRevenueRecognized
        {
            BookingId = bookingId, BranchId = Guid.NewGuid(), CustomerId = null, TourId = Guid.NewGuid(),
            TourTypeCode = "GROUP", Subtotal = subtotal, AddonsTotal = addons, Discount = discount,
            TaxTotal = tax, Total = total, Currency = currency, SupplierId = supplierId, SupplierPercentage = supplierPct
        };
    }

    [Fact]
    public async Task Revenue_Creates_Snapshot_Journal_And_RevenueDaily()
    {
        var (c, db) = Build();
        var id = Guid.NewGuid();
        await c.HandleRevenueAsync(Revenue(id, subtotal: 200m, addons: 50m, discount: 30m, tax: 22m));

        var snap = await db.BookingFinancialSnapshots.SingleAsync();
        Assert.Equal(250m, snap.Gross);
        Assert.Equal(220m, snap.Net);       // gross - discount
        Assert.Equal(242m, snap.Due);       // total = net + tax
        Assert.Equal("Recognized", snap.Status);

        var entry = await db.JournalEntries.Include(e => e.Lines).SingleAsync();
        Assert.Equal(entry.Lines.Sum(l => l.ReportingDebit), entry.Lines.Sum(l => l.ReportingCredit));

        var daily = await db.RevenueDaily.SingleAsync();
        Assert.Equal(220m, daily.Recognized);
    }

    [Fact]
    public async Task Revenue_Redelivery_Does_Not_Double_Post()
    {
        var (c, db) = Build();
        var id = Guid.NewGuid();
        await c.HandleRevenueAsync(Revenue(id));
        // a fresh event id for the same booking (a redelivery) must be ignored by the business-key guard
        await c.HandleRevenueAsync(Revenue(id));

        Assert.Equal(1, await db.BookingFinancialSnapshots.CountAsync());
        Assert.Equal(1, await db.JournalEntries.CountAsync());
    }

    [Fact]
    public async Task Revenue_WithSupplier_Accrues_Settlement_And_Posts_Supplier_Entry()
    {
        var (c, db) = Build();
        var supplierId = Guid.NewGuid();
        await c.HandleRevenueAsync(Revenue(Guid.NewGuid(), subtotal: 100m, addons: 100m, supplierId: supplierId, supplierPct: 20m));

        var settlement = await db.SupplierSettlements.SingleAsync();
        Assert.Equal(40m, settlement.AccruedAmount);     // 200 * 20%
        Assert.Equal(2, await db.JournalEntries.CountAsync()); // revenue + supplier accrual
        var snap = await db.BookingFinancialSnapshots.SingleAsync();
        Assert.Equal(40m, snap.SupplierCost);
        Assert.Equal(160m, snap.Margin);                 // net(200) - cost(40)
    }

    [Fact]
    public async Task Cancellation_Reverses_Revenue_And_Marks_Snapshot()
    {
        var (c, db) = Build();
        var id = Guid.NewGuid();
        var rev = Revenue(id, subtotal: 100m, tax: 14m);
        await c.HandleRevenueAsync(rev);

        await c.HandleCancelledAsync(new BookingCancelled
        {
            BookingId = id, BranchId = rev.BranchId, RefundAmount = 0m, Currency = "USD"
        });

        var snap = await db.BookingFinancialSnapshots.SingleAsync();
        Assert.Equal("Cancelled", snap.Status);

        var daily = await db.RevenueDaily.SingleAsync();
        Assert.Equal(0m, daily.Recognized);   // recognition then reversal nets to zero

        // net ledger effect per account is zero
        var lines = await db.JournalLines.ToListAsync();
        foreach (var g in lines.GroupBy(l => l.AccountId))
            Assert.Equal(0m, g.Sum(l => l.ReportingDebit - l.ReportingCredit));
    }

    [Fact]
    public async Task Cancellation_Redelivery_Is_Idempotent()
    {
        var (c, db) = Build();
        var id = Guid.NewGuid();
        var rev = Revenue(id);
        await c.HandleRevenueAsync(rev);
        var cancel = new BookingCancelled { BookingId = id, BranchId = rev.BranchId, RefundAmount = 0m, Currency = "USD" };
        await c.HandleCancelledAsync(cancel);
        await c.HandleCancelledAsync(cancel);   // second time is a no-op (status already Cancelled)

        // one recognition + one reversal only
        Assert.Equal(2, await db.JournalEntries.CountAsync());
    }

    [Fact]
    public async Task ForeignCurrency_Uses_Seeded_Rate()
    {
        var (c, db) = Build();
        db.CurrencyRates.Add(new CurrencyRate
        {
            Id = Guid.NewGuid(), FromCurrency = "EUR", ToCurrency = FinanceConstants.ReportingCurrency,
            Rate = 1.10m, AsOfUtc = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });
        await db.SaveChangesAsync();

        await c.HandleRevenueAsync(Revenue(Guid.NewGuid(), subtotal: 100m, currency: "EUR"));

        var arLine = (await db.JournalLines.ToListAsync())
            .Single(l => l.AccountId == ChartOfAccounts.AccountsReceivable);
        Assert.Equal(110m, arLine.ReportingDebit);
    }

    [Fact]
    public async Task RefundIssued_Posts_Refund_And_Updates_Daily()
    {
        var (c, db) = Build();
        await c.HandleRefundAsync(new RefundIssued
        {
            BookingId = Guid.NewGuid(), BranchId = Guid.NewGuid(), RefundAmount = 60m, Currency = "USD"
        });

        var daily = await db.RevenueDaily.SingleAsync();
        Assert.Equal(60m, daily.Refunds);
        var entry = await db.JournalEntries.Include(e => e.Lines).SingleAsync();
        Assert.Equal(entry.Lines.Sum(l => l.ReportingDebit), entry.Lines.Sum(l => l.ReportingCredit));
    }
}
