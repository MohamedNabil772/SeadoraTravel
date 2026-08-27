using MassTransit;
using Microsoft.EntityFrameworkCore;
using Seadora.Common.Messaging.Idempotency;
using Seadora.Contracts.Events;
using Seadora.Finance.Application.Common;
using Seadora.Finance.Application.Common.Interfaces;
using Seadora.Finance.Domain.Entities;
using Seadora.Finance.Domain.Enums;
using Seadora.Finance.Domain.Posting;

namespace Seadora.Finance.Application.Integration;

/// <summary>
/// Posts balanced journal entries and projects the finance read-models in reaction to booking events.
/// Idempotent on <c>evt.Id</c>, with a business-key guard so redelivery under a fresh id never double-posts.
/// </summary>
public sealed class FinanceEventConsumers
    : IConsumer<BookingRevenueRecognized>, IConsumer<BookingCancelled>, IConsumer<RefundIssued>
{
    public const string RevenueConsumer = nameof(FinanceEventConsumers) + ":" + nameof(BookingRevenueRecognized);
    public const string CancelledConsumer = nameof(FinanceEventConsumers) + ":" + nameof(BookingCancelled);
    public const string RefundConsumer = nameof(FinanceEventConsumers) + ":" + nameof(RefundIssued);

    private readonly IFinanceDbContext _db;
    private readonly IIdempotentConsumer _idem;

    public FinanceEventConsumers(IFinanceDbContext db, IIdempotentConsumer idem)
    {
        _db = db;
        _idem = idem;
    }

    public Task Consume(ConsumeContext<BookingRevenueRecognized> ctx) => HandleRevenueAsync(ctx.Message, ctx.CancellationToken);
    public Task Consume(ConsumeContext<BookingCancelled> ctx) => HandleCancelledAsync(ctx.Message, ctx.CancellationToken);
    public Task Consume(ConsumeContext<RefundIssued> ctx) => HandleRefundAsync(ctx.Message, ctx.CancellationToken);

    public async Task HandleRevenueAsync(BookingRevenueRecognized evt, CancellationToken ct = default)
    {
        if (await _idem.AlreadyProcessed(evt.Id, RevenueConsumer, ct)) return;

        // business-key guard: a redelivery under a new evt.Id must not recognize revenue twice.
        var existing = await _db.BookingFinancialSnapshots.FirstOrDefaultAsync(s => s.BookingId == evt.BookingId, ct);
        if (existing is not null)
        {
            await _idem.MarkProcessed(evt.Id, RevenueConsumer, ct);
            return;
        }

        var facts = new RevenueFacts(evt.BookingId, evt.BranchId, evt.CustomerId, evt.TourId, evt.TourTypeCode,
            evt.Subtotal, evt.AddonsTotal, evt.Discount, evt.TaxTotal, evt.Total, evt.Currency,
            evt.SupplierId, evt.SupplierPercentage, evt.OccurredUtc, evt.Id.ToString());

        var fx = await FxResolver.ResolveRateAsync(_db, evt.Currency, evt.OccurredUtc, ct);

        _db.JournalEntries.Add(LedgerPosting.RevenueRecognition(facts, fx));

        var supplierCost = LedgerPosting.SupplierCostOf(facts);
        var supplierEntry = LedgerPosting.SupplierAccrual(facts, fx);
        if (supplierEntry is not null)
        {
            _db.JournalEntries.Add(supplierEntry);
            await AccrueSupplierAsync(evt.SupplierId!.Value, evt.BranchId, evt.OccurredUtc, supplierCost, evt.Currency, ct);
        }

        var net = facts.Gross - facts.Discount;
        _db.BookingFinancialSnapshots.Add(new BookingFinancialSnapshot
        {
            Id = Guid.NewGuid(),
            BookingId = evt.BookingId,
            BranchId = evt.BranchId,
            CustomerId = evt.CustomerId,
            TourId = evt.TourId,
            TourTypeCode = evt.TourTypeCode,
            Gross = facts.Gross,
            Discount = facts.Discount,
            Tax = facts.TaxTotal,
            Net = net,
            SupplierCost = supplierCost,
            Margin = net - supplierCost,
            Paid = 0m,
            Due = facts.Total,
            Currency = evt.Currency,
            Status = "Recognized",
            BookingDateUtc = evt.OccurredUtc,
            UpdatedUtc = DateTime.UtcNow
        });

        await AdjustRevenueDailyAsync(evt.BranchId, evt.OccurredUtc.Date, evt.Currency,
            recognized: net, collected: 0m, refunds: 0m, supplierCost: supplierCost, margin: net - supplierCost, ct);

        await _db.SaveChangesAsync(ct);
        await _idem.MarkProcessed(evt.Id, RevenueConsumer, ct);
    }

    public async Task HandleCancelledAsync(BookingCancelled evt, CancellationToken ct = default)
    {
        if (await _idem.AlreadyProcessed(evt.Id, CancelledConsumer, ct)) return;

        var snap = await _db.BookingFinancialSnapshots.FirstOrDefaultAsync(s => s.BookingId == evt.BookingId, ct);
        if (snap is null || snap.Status == "Cancelled")
        {
            await _idem.MarkProcessed(evt.Id, CancelledConsumer, ct);
            return;
        }

        var fx = await FxResolver.ResolveRateAsync(_db, snap.Currency, evt.OccurredUtc, ct);

        _db.JournalEntries.Add(LedgerPosting.RevenueReversal(snap, evt.OccurredUtc, snap.Currency, fx, evt.Id.ToString()));

        var supplierReversal = LedgerPosting.SupplierReversal(snap, evt.OccurredUtc, snap.Currency, fx, evt.Id.ToString());
        if (supplierReversal is not null)
        {
            _db.JournalEntries.Add(supplierReversal);
            // ponytail: settlement decrement needs the supplier id on the snapshot; it is not stored today,
            // so we reverse the ledger but leave the (already accrued) settlement for the accountant to net off.
        }

        await AdjustRevenueDailyAsync(snap.BranchId, snap.BookingDateUtc.Date, snap.Currency,
            recognized: -snap.Net, collected: 0m, refunds: 0m, supplierCost: -snap.SupplierCost,
            margin: -(snap.Net - snap.SupplierCost), ct);

        if (evt.RefundAmount > 0)
        {
            _db.JournalEntries.Add(LedgerPosting.Refund(evt.BookingId, evt.BranchId, evt.RefundAmount,
                evt.Currency, fx, evt.OccurredUtc, evt.Id.ToString() + ":refund"));
            await AdjustRevenueDailyAsync(snap.BranchId, evt.OccurredUtc.Date, evt.Currency,
                recognized: 0m, collected: 0m, refunds: evt.RefundAmount, supplierCost: 0m, margin: 0m, ct);
            snap.Paid = Math.Max(0m, snap.Paid - evt.RefundAmount);
            snap.Due = Math.Max(0m, (snap.Net + snap.Tax) - snap.Paid);
        }

        snap.Status = "Cancelled";
        snap.UpdatedUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        await _idem.MarkProcessed(evt.Id, CancelledConsumer, ct);
    }

    public async Task HandleRefundAsync(RefundIssued evt, CancellationToken ct = default)
    {
        if (await _idem.AlreadyProcessed(evt.Id, RefundConsumer, ct)) return;

        if (evt.RefundAmount <= 0)
        {
            await _idem.MarkProcessed(evt.Id, RefundConsumer, ct);
            return;
        }

        var fx = await FxResolver.ResolveRateAsync(_db, evt.Currency, evt.OccurredUtc, ct);

        _db.JournalEntries.Add(LedgerPosting.Refund(evt.BookingId, evt.BranchId, evt.RefundAmount,
            evt.Currency, fx, evt.OccurredUtc, evt.Id.ToString()));

        await AdjustRevenueDailyAsync(evt.BranchId, evt.OccurredUtc.Date, evt.Currency,
            recognized: 0m, collected: 0m, refunds: evt.RefundAmount, supplierCost: 0m, margin: 0m, ct);

        var snap = await _db.BookingFinancialSnapshots.FirstOrDefaultAsync(s => s.BookingId == evt.BookingId, ct);
        if (snap is not null)
        {
            snap.Paid = Math.Max(0m, snap.Paid - evt.RefundAmount);
            snap.Due = Math.Max(0m, (snap.Net + snap.Tax) - snap.Paid);
            snap.UpdatedUtc = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync(ct);
        await _idem.MarkProcessed(evt.Id, RefundConsumer, ct);
    }

    private async Task AccrueSupplierAsync(Guid supplierId, Guid branchId, DateTime occurredUtc,
        decimal cost, string currency, CancellationToken ct)
    {
        var periodStart = new DateTime(occurredUtc.Year, occurredUtc.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var periodEnd = periodStart.AddMonths(1);
        var settlement = await _db.SupplierSettlements.FirstOrDefaultAsync(
            s => s.SupplierId == supplierId && s.BranchId == branchId
                 && s.PeriodStart == periodStart && s.PeriodEnd == periodEnd, ct);
        if (settlement is null)
        {
            settlement = new SupplierSettlement
            {
                Id = Guid.NewGuid(),
                SupplierId = supplierId,
                BranchId = branchId,
                PeriodStart = periodStart,
                PeriodEnd = periodEnd,
                AccruedAmount = 0m,
                PaidAmount = 0m,
                Currency = currency,
                Status = SettlementStatus.Accrued
            };
            _db.SupplierSettlements.Add(settlement);
        }
        settlement.AccruedAmount += cost;
        settlement.Status = settlement.PaidAmount >= settlement.AccruedAmount && settlement.AccruedAmount > 0
            ? SettlementStatus.Paid
            : settlement.PaidAmount > 0 ? SettlementStatus.PartiallyPaid : SettlementStatus.Accrued;
    }

    private async Task AdjustRevenueDailyAsync(Guid branchId, DateTime day, string currency,
        decimal recognized, decimal collected, decimal refunds, decimal supplierCost, decimal margin, CancellationToken ct)
    {
        var row = await _db.RevenueDaily.FirstOrDefaultAsync(
            r => r.BranchId == branchId && r.Day == day && r.Currency == currency, ct);
        if (row is null)
        {
            row = new RevenueDaily
            {
                Id = Guid.NewGuid(),
                BranchId = branchId,
                Day = day,
                Currency = currency
            };
            _db.RevenueDaily.Add(row);
        }
        row.Recognized += recognized;
        row.Collected += collected;
        row.Refunds += refunds;
        row.SupplierCost += supplierCost;
        row.Margin += margin;
    }
}
