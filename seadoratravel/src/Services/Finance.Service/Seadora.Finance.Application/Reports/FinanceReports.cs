using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Finance.Application.Common.Interfaces;
using Seadora.Finance.Domain;

namespace Seadora.Finance.Application.Reports;

/// <summary>Common report filter. All fields optional; only set ones are applied.</summary>
public record ReportFilter(
    DateTime? From = null,
    DateTime? To = null,
    Guid? BranchId = null,
    string? Currency = null,
    int Page = 1,
    int PageSize = 50);

public record PagedResult<T>(IReadOnlyList<T> Items, int Total, int Page, int PageSize);

// ---- 1. General Ledger / Journal ----
public record LedgerLineDto(Guid EntryId, DateTime OccurredUtc, string Description, Guid BranchId,
    Guid? BookingId, string AccountCode, string AccountName, decimal Debit, decimal Credit,
    string Currency, decimal ReportingDebit, decimal ReportingCredit);

public record GeneralLedgerQuery(ReportFilter Filter, Guid? AccountId = null) : IRequest<PagedResult<LedgerLineDto>>;

public class GeneralLedgerQueryHandler : IRequestHandler<GeneralLedgerQuery, PagedResult<LedgerLineDto>>
{
    private readonly IFinanceDbContext _db;
    public GeneralLedgerQueryHandler(IFinanceDbContext db) => _db = db;

    public async Task<PagedResult<LedgerLineDto>> Handle(GeneralLedgerQuery q, CancellationToken ct)
    {
        var f = q.Filter;
        var entriesQuery = _db.JournalEntries.Include(e => e.Lines).AsQueryable();
        if (f.From is not null) entriesQuery = entriesQuery.Where(e => e.OccurredUtc >= f.From);
        if (f.To is not null) entriesQuery = entriesQuery.Where(e => e.OccurredUtc <= f.To);
        if (f.BranchId is not null) entriesQuery = entriesQuery.Where(e => e.BranchId == f.BranchId);

        var entries = await entriesQuery.OrderByDescending(e => e.OccurredUtc).ToListAsync(ct);
        var accounts = await _db.LedgerAccounts.ToDictionaryAsync(a => a.Id, ct);

        var rows = new List<LedgerLineDto>();
        foreach (var e in entries)
        {
            foreach (var l in e.Lines)
            {
                if (q.AccountId is not null && l.AccountId != q.AccountId) continue;
                if (f.Currency is not null && !string.Equals(l.Currency, f.Currency, StringComparison.OrdinalIgnoreCase)) continue;
                var acc = accounts.GetValueOrDefault(l.AccountId);
                rows.Add(new LedgerLineDto(e.Id, e.OccurredUtc, e.Description, e.BranchId, e.BookingId,
                    acc?.Code ?? "?", acc?.Name ?? "?", l.Debit, l.Credit, l.Currency, l.ReportingDebit, l.ReportingCredit));
            }
        }

        var total = rows.Count;
        var page = Math.Max(1, f.Page);
        var pageSize = Math.Clamp(f.PageSize, 1, 500);
        var items = rows.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return new PagedResult<LedgerLineDto>(items, total, page, pageSize);
    }
}

// ---- 2. Trial Balance ----
public record TrialBalanceRow(string AccountCode, string AccountName, string AccountType,
    decimal TotalDebit, decimal TotalCredit, decimal Balance);

public record TrialBalanceQuery(ReportFilter Filter) : IRequest<IReadOnlyList<TrialBalanceRow>>;

public class TrialBalanceQueryHandler : IRequestHandler<TrialBalanceQuery, IReadOnlyList<TrialBalanceRow>>
{
    private readonly IFinanceDbContext _db;
    public TrialBalanceQueryHandler(IFinanceDbContext db) => _db = db;

    public async Task<IReadOnlyList<TrialBalanceRow>> Handle(TrialBalanceQuery q, CancellationToken ct)
    {
        var f = q.Filter;
        var entriesQuery = _db.JournalEntries.Include(e => e.Lines).AsQueryable();
        if (f.From is not null) entriesQuery = entriesQuery.Where(e => e.OccurredUtc >= f.From);
        if (f.To is not null) entriesQuery = entriesQuery.Where(e => e.OccurredUtc <= f.To);
        if (f.BranchId is not null) entriesQuery = entriesQuery.Where(e => e.BranchId == f.BranchId);

        var entries = await entriesQuery.ToListAsync(ct);
        var accounts = await _db.LedgerAccounts.ToDictionaryAsync(a => a.Id, ct);

        var totals = new Dictionary<Guid, (decimal Debit, decimal Credit)>();
        foreach (var e in entries)
            foreach (var l in e.Lines)
            {
                var t = totals.GetValueOrDefault(l.AccountId);
                totals[l.AccountId] = (t.Debit + l.ReportingDebit, t.Credit + l.ReportingCredit);
            }

        // ponytail: trial balance is driven by accounts that actually moved; chart metadata only
        // supplies display code/name/type, so a missing seed row never blanks the report.
        return totals
            .Select(kv =>
            {
                var a = accounts.GetValueOrDefault(kv.Key);
                return new TrialBalanceRow(a?.Code ?? "?", a?.Name ?? kv.Key.ToString(),
                    a?.Type.ToString() ?? "?", kv.Value.Debit, kv.Value.Credit, kv.Value.Debit - kv.Value.Credit);
            })
            .OrderBy(r => r.AccountCode)
            .ToList();
    }
}

// ---- 3. Profit & Loss ----
public record PlByGroup(string Key, decimal Net, decimal SupplierCost, decimal Margin);
public record ProfitAndLossDto(decimal Gross, decimal Discounts, decimal Net, decimal SupplierCost,
    decimal Refunds, decimal NetProfit, IReadOnlyList<PlByGroup> ByTourType, IReadOnlyList<PlByGroup> ByBranch);

public record ProfitAndLossQuery(ReportFilter Filter) : IRequest<ProfitAndLossDto>;

public class ProfitAndLossQueryHandler : IRequestHandler<ProfitAndLossQuery, ProfitAndLossDto>
{
    private readonly IFinanceDbContext _db;
    public ProfitAndLossQueryHandler(IFinanceDbContext db) => _db = db;

    public async Task<ProfitAndLossDto> Handle(ProfitAndLossQuery q, CancellationToken ct)
    {
        var f = q.Filter;
        var snaps = await FinanceReportFilters.Snapshots(_db, f).Where(s => s.Status != "Cancelled").ToListAsync(ct);
        var refunds = await FinanceReportFilters.Daily(_db, f).SumAsync(r => (decimal?)r.Refunds, ct) ?? 0m;

        var gross = snaps.Sum(s => s.Gross);
        var discounts = snaps.Sum(s => s.Discount);
        var net = snaps.Sum(s => s.Net);
        var supplierCost = snaps.Sum(s => s.SupplierCost);
        var netProfit = net - supplierCost - refunds;

        var byTourType = snaps.GroupBy(s => s.TourTypeCode ?? "UNSET")
            .Select(g => new PlByGroup(g.Key, g.Sum(s => s.Net), g.Sum(s => s.SupplierCost), g.Sum(s => s.Margin)))
            .OrderByDescending(x => x.Margin).ToList();
        var byBranch = snaps.GroupBy(s => s.BranchId)
            .Select(g => new PlByGroup(g.Key.ToString(), g.Sum(s => s.Net), g.Sum(s => s.SupplierCost), g.Sum(s => s.Margin)))
            .ToList();

        return new ProfitAndLossDto(gross, discounts, net, supplierCost, refunds, netProfit, byTourType, byBranch);
    }
}

// ---- 4. Revenue report (recognized vs collected) ----
public record RevenueRow(DateTime Day, Guid BranchId, string Currency, decimal Recognized, decimal Collected);
public record RevenueReportDto(decimal TotalRecognized, decimal TotalCollected, IReadOnlyList<RevenueRow> Series);

public record RevenueReportQuery(ReportFilter Filter) : IRequest<RevenueReportDto>;

public class RevenueReportQueryHandler : IRequestHandler<RevenueReportQuery, RevenueReportDto>
{
    private readonly IFinanceDbContext _db;
    public RevenueReportQueryHandler(IFinanceDbContext db) => _db = db;

    public async Task<RevenueReportDto> Handle(RevenueReportQuery q, CancellationToken ct)
    {
        var rows = await FinanceReportFilters.Daily(_db, q.Filter)
            .OrderBy(r => r.Day)
            .Select(r => new RevenueRow(r.Day, r.BranchId, r.Currency, r.Recognized, r.Collected))
            .ToListAsync(ct);
        return new RevenueReportDto(rows.Sum(r => r.Recognized), rows.Sum(r => r.Collected), rows);
    }
}

// ---- 5. Accounts Receivable aging ----
public record ArAgingRow(Guid BookingId, Guid BranchId, string Currency, decimal Due, int AgeDays,
    string Bucket, DateTime BookingDateUtc);
public record ArAgingDto(decimal Bucket0_30, decimal Bucket31_60, decimal Bucket61_90, decimal Bucket90Plus,
    decimal Total, IReadOnlyList<ArAgingRow> Items);

public record ArAgingQuery(ReportFilter Filter, DateTime? AsOf = null) : IRequest<ArAgingDto>;

public class ArAgingQueryHandler : IRequestHandler<ArAgingQuery, ArAgingDto>
{
    private readonly IFinanceDbContext _db;
    public ArAgingQueryHandler(IFinanceDbContext db) => _db = db;

    public async Task<ArAgingDto> Handle(ArAgingQuery q, CancellationToken ct)
    {
        var asOf = q.AsOf ?? DateTime.UtcNow;
        var snaps = await FinanceReportFilters.Snapshots(_db, q.Filter)
            .Where(s => s.Status != "Cancelled" && s.Due > 0)
            .ToListAsync(ct);

        var rows = snaps.Select(s =>
        {
            var age = Math.Max(0, (int)(asOf.Date - s.BookingDateUtc.Date).TotalDays);
            var bucket = age <= 30 ? "0-30" : age <= 60 ? "31-60" : age <= 90 ? "61-90" : "90+";
            return new ArAgingRow(s.BookingId, s.BranchId, s.Currency, s.Due, age, bucket, s.BookingDateUtc);
        }).OrderByDescending(r => r.AgeDays).ToList();

        return new ArAgingDto(
            rows.Where(r => r.Bucket == "0-30").Sum(r => r.Due),
            rows.Where(r => r.Bucket == "31-60").Sum(r => r.Due),
            rows.Where(r => r.Bucket == "61-90").Sum(r => r.Due),
            rows.Where(r => r.Bucket == "90+").Sum(r => r.Due),
            rows.Sum(r => r.Due),
            rows);
    }
}

// ---- 6. Supplier payables / settlement ----
public record SupplierPayableRow(Guid SupplierId, Guid BranchId, DateTime PeriodStart, DateTime PeriodEnd,
    decimal Accrued, decimal Paid, decimal Due, string Status, string Currency);

public record SupplierPayablesQuery(ReportFilter Filter) : IRequest<IReadOnlyList<SupplierPayableRow>>;

public class SupplierPayablesQueryHandler : IRequestHandler<SupplierPayablesQuery, IReadOnlyList<SupplierPayableRow>>
{
    private readonly IFinanceDbContext _db;
    public SupplierPayablesQueryHandler(IFinanceDbContext db) => _db = db;

    public async Task<IReadOnlyList<SupplierPayableRow>> Handle(SupplierPayablesQuery q, CancellationToken ct)
    {
        var f = q.Filter;
        var query = _db.SupplierSettlements.AsQueryable();
        if (f.From is not null) query = query.Where(s => s.PeriodEnd >= f.From);
        if (f.To is not null) query = query.Where(s => s.PeriodStart <= f.To);
        if (f.BranchId is not null) query = query.Where(s => s.BranchId == f.BranchId);
        if (f.Currency is not null) query = query.Where(s => s.Currency == f.Currency);

        return await query.OrderBy(s => s.PeriodStart)
            .Select(s => new SupplierPayableRow(s.SupplierId, s.BranchId, s.PeriodStart, s.PeriodEnd,
                s.AccruedAmount, s.PaidAmount, s.AccruedAmount - s.PaidAmount, s.Status.ToString(), s.Currency))
            .ToListAsync(ct);
    }
}

// ---- 7. Receipts & payments subledger ----
public record PaymentRow(Guid Id, Guid BookingId, Guid BranchId, decimal Amount, string Currency,
    string Method, string? Reference, DateTime ReceivedUtc, bool Reconciled);

public record ReceiptsQuery(ReportFilter Filter) : IRequest<PagedResult<PaymentRow>>;

public class ReceiptsQueryHandler : IRequestHandler<ReceiptsQuery, PagedResult<PaymentRow>>
{
    private readonly IFinanceDbContext _db;
    public ReceiptsQueryHandler(IFinanceDbContext db) => _db = db;

    public async Task<PagedResult<PaymentRow>> Handle(ReceiptsQuery q, CancellationToken ct)
    {
        var f = q.Filter;
        var query = _db.Payments.AsQueryable();
        if (f.From is not null) query = query.Where(p => p.ReceivedUtc >= f.From);
        if (f.To is not null) query = query.Where(p => p.ReceivedUtc <= f.To);
        if (f.BranchId is not null) query = query.Where(p => p.BranchId == f.BranchId);
        if (f.Currency is not null) query = query.Where(p => p.Currency == f.Currency);

        var total = await query.CountAsync(ct);
        var page = Math.Max(1, f.Page);
        var pageSize = Math.Clamp(f.PageSize, 1, 500);
        var items = await query.OrderByDescending(p => p.ReceivedUtc)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(p => new PaymentRow(p.Id, p.BookingId, p.BranchId, p.Amount, p.Currency,
                p.Method.ToString(), p.Reference, p.ReceivedUtc, p.ReconciledUtc != null))
            .ToListAsync(ct);
        return new PagedResult<PaymentRow>(items, total, page, pageSize);
    }
}

// ---- 8. Refunds report ----
public record RefundRow(DateTime Day, Guid BranchId, string Currency, decimal Refunds);
public record RefundsReportDto(decimal Total, IReadOnlyList<RefundRow> Series);

public record RefundsReportQuery(ReportFilter Filter) : IRequest<RefundsReportDto>;

public class RefundsReportQueryHandler : IRequestHandler<RefundsReportQuery, RefundsReportDto>
{
    private readonly IFinanceDbContext _db;
    public RefundsReportQueryHandler(IFinanceDbContext db) => _db = db;

    public async Task<RefundsReportDto> Handle(RefundsReportQuery q, CancellationToken ct)
    {
        var rows = await FinanceReportFilters.Daily(_db, q.Filter)
            .Where(r => r.Refunds != 0)
            .OrderBy(r => r.Day)
            .Select(r => new RefundRow(r.Day, r.BranchId, r.Currency, r.Refunds))
            .ToListAsync(ct);
        return new RefundsReportDto(rows.Sum(r => r.Refunds), rows);
    }
}

// ---- 9. Tax collected ----
public record TaxRow(Guid BranchId, string Currency, decimal TaxCollected);
public record TaxReportDto(decimal Total, IReadOnlyList<TaxRow> ByBranch);

public record TaxReportQuery(ReportFilter Filter) : IRequest<TaxReportDto>;

public class TaxReportQueryHandler : IRequestHandler<TaxReportQuery, TaxReportDto>
{
    private readonly IFinanceDbContext _db;
    public TaxReportQueryHandler(IFinanceDbContext db) => _db = db;

    public async Task<TaxReportDto> Handle(TaxReportQuery q, CancellationToken ct)
    {
        var snaps = await FinanceReportFilters.Snapshots(_db, q.Filter)
            .Where(s => s.Status != "Cancelled" && s.Tax != 0)
            .ToListAsync(ct);
        var byBranch = snaps.GroupBy(s => new { s.BranchId, s.Currency })
            .Select(g => new TaxRow(g.Key.BranchId, g.Key.Currency, g.Sum(s => s.Tax)))
            .OrderBy(r => r.BranchId).ToList();
        return new TaxReportDto(byBranch.Sum(r => r.TaxCollected), byBranch);
    }
}

// ---- shared filter helpers ----
internal static class FinanceReportFilters
{
    public static IQueryable<Domain.Entities.BookingFinancialSnapshot> Snapshots(IFinanceDbContext db, ReportFilter f)
    {
        var q = db.BookingFinancialSnapshots.AsQueryable();
        if (f.From is not null) q = q.Where(s => s.BookingDateUtc >= f.From);
        if (f.To is not null) q = q.Where(s => s.BookingDateUtc <= f.To);
        if (f.BranchId is not null) q = q.Where(s => s.BranchId == f.BranchId);
        if (f.Currency is not null) q = q.Where(s => s.Currency == f.Currency);
        return q;
    }

    public static IQueryable<Domain.Entities.RevenueDaily> Daily(IFinanceDbContext db, ReportFilter f)
    {
        var q = db.RevenueDaily.AsQueryable();
        if (f.From is not null) q = q.Where(r => r.Day >= f.From!.Value.Date);
        if (f.To is not null) q = q.Where(r => r.Day <= f.To!.Value.Date);
        if (f.BranchId is not null) q = q.Where(r => r.BranchId == f.BranchId);
        if (f.Currency is not null) q = q.Where(r => r.Currency == f.Currency);
        return q;
    }
}
