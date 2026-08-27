using Microsoft.EntityFrameworkCore;
using Seadora.Common.Messaging.Outbox;
using Seadora.Contracts.Events;
using Seadora.Finance.Application.Payments.Commands.RecordPayment;
using Seadora.Finance.Domain.Entities;
using Seadora.Finance.Domain.Enums;
using Seadora.Finance.Infrastructure.Persistence;

namespace Seadora.UnitTests;

public class FinancePaymentTests
{
    private static (RecordPaymentCommandHandler Handler, FinanceDbContext Db) Build()
    {
        var options = new DbContextOptionsBuilder<FinanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var db = new FinanceDbContext(options);
        var handler = new RecordPaymentCommandHandler(db, new OutboxWriter(db));
        return (handler, db);
    }

    private static async Task<BookingFinancialSnapshot> SeedSnapshotAsync(FinanceDbContext db,
        Guid bookingId, decimal net = 220m, decimal tax = 22m, string currency = "USD")
    {
        var snap = new BookingFinancialSnapshot
        {
            Id = Guid.NewGuid(),
            BookingId = bookingId,
            BranchId = Guid.NewGuid(),
            TourId = Guid.NewGuid(),
            Gross = net,
            Discount = 0m,
            Tax = tax,
            Net = net,
            SupplierCost = 0m,
            Margin = net,
            Paid = 0m,
            Due = net + tax,
            Currency = currency,
            Status = "Recognized",
            BookingDateUtc = DateTime.UtcNow,
            UpdatedUtc = DateTime.UtcNow
        };
        db.BookingFinancialSnapshots.Add(snap);
        await db.SaveChangesAsync();
        return snap;
    }

    [Fact]
    public async Task Payment_Updates_Snapshot_Paid_And_Due()
    {
        var (handler, db) = Build();
        var bookingId = Guid.NewGuid();
        await SeedSnapshotAsync(db, bookingId, net: 220m, tax: 22m); // total 242

        await handler.Handle(new RecordPaymentCommand(bookingId, 100m, PaymentMethod.Card, "ref-1", null, "acct@x"), default);

        var snap = await db.BookingFinancialSnapshots.SingleAsync();
        Assert.Equal(100m, snap.Paid);
        Assert.Equal(142m, snap.Due);
    }

    [Fact]
    public async Task Payment_Posts_Balanced_Journal_Entry()
    {
        var (handler, db) = Build();
        var bookingId = Guid.NewGuid();
        await SeedSnapshotAsync(db, bookingId);

        await handler.Handle(new RecordPaymentCommand(bookingId, 50m, PaymentMethod.Cash, null, null, null), default);

        var entry = await db.JournalEntries.Include(e => e.Lines).SingleAsync();
        Assert.Equal(2, entry.Lines.Count);
        Assert.Equal(entry.Lines.Sum(l => l.ReportingDebit), entry.Lines.Sum(l => l.ReportingCredit));
        Assert.Equal(50m, entry.Lines.Sum(l => l.Debit));
    }

    [Fact]
    public async Task Payment_Increments_RevenueDaily_Collected()
    {
        var (handler, db) = Build();
        var bookingId = Guid.NewGuid();
        var received = new DateTime(2026, 3, 5, 10, 0, 0, DateTimeKind.Utc);
        await SeedSnapshotAsync(db, bookingId);

        await handler.Handle(new RecordPaymentCommand(bookingId, 75m, PaymentMethod.Bank, null, received, null), default);

        var daily = await db.RevenueDaily.SingleAsync();
        Assert.Equal(received.Date, daily.Day);
        Assert.Equal(75m, daily.Collected);
    }

    [Fact]
    public async Task Payment_Emits_PaymentRecorded_With_Cumulative()
    {
        var (handler, db) = Build();
        var bookingId = Guid.NewGuid();
        await SeedSnapshotAsync(db, bookingId, net: 220m, tax: 22m);

        await handler.Handle(new RecordPaymentCommand(bookingId, 100m, PaymentMethod.Card, null, null, null), default);
        await handler.Handle(new RecordPaymentCommand(bookingId, 40m, PaymentMethod.Card, null, null, null), default);

        var messages = await db.OutboxMessages.ToListAsync();
        Assert.Equal(2, messages.Count);
        Assert.All(messages, m => Assert.Contains(nameof(PaymentRecorded), m.Type));

        var snap = await db.BookingFinancialSnapshots.SingleAsync();
        Assert.Equal(140m, snap.Paid);
        Assert.Equal(102m, snap.Due);
    }

    [Fact]
    public async Task Payment_For_Unknown_Booking_Throws()
    {
        var (handler, _) = Build();
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new RecordPaymentCommand(Guid.NewGuid(), 10m, PaymentMethod.Cash, null, null, null), default));
    }

    [Fact]
    public async Task Payment_Records_Persisted_Payment_Row()
    {
        var (handler, db) = Build();
        var bookingId = Guid.NewGuid();
        await SeedSnapshotAsync(db, bookingId, currency: "EUR");

        var id = await handler.Handle(new RecordPaymentCommand(bookingId, 60m, PaymentMethod.Other, "wire-9", null, "owner@x"), default);

        var payment = await db.Payments.SingleAsync();
        Assert.Equal(id, payment.Id);
        Assert.Equal(60m, payment.Amount);
        Assert.Equal("EUR", payment.Currency);
        Assert.Equal("wire-9", payment.Reference);
        Assert.Equal("owner@x", payment.CreatedBy);
    }
}
