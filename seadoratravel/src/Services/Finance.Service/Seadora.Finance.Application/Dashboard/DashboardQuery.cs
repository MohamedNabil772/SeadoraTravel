using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Finance.Application.Common.Interfaces;

namespace Seadora.Finance.Application.Dashboard;

/// <summary>
/// Owner dashboard aggregates (§9 of the plan): KPI cards, growth deltas, trend series, and the
/// top tour-types / per-branch splits. Read-only over the finance read-models - no journal scans.
/// </summary>
public record DashboardQuery(
    DateTime? From = null,
    DateTime? To = null,
    Guid? BranchId = null,
    string? Currency = null,
    string Granularity = "day") : IRequest<DashboardDto>;

public record KpiCards(
    decimal RevenueRecognized,
    decimal RevenueCollected,
    decimal SupplierCost,
    decimal NetProfit,
    decimal GrossMarginPct,
    decimal OutstandingAr,
    decimal Refunds,
    int Bookings,
    decimal AverageBookingValue,
    decimal CancellationRatePct,
    decimal RefundRatePct);

public record GrowthDto(decimal RevenueMoMPct, decimal RevenuePrevPeriod, decimal RevenueThisPeriod);

public record TrendPoint(string Period, decimal Recognized, decimal Collected, decimal Margin, decimal Refunds);

public record RankRow(string Key, decimal Revenue, decimal Margin, int Bookings);

public record BranchSplitRow(Guid BranchId, decimal Revenue, decimal SupplierCost, decimal Margin);

public record DashboardDto(
    KpiCards Kpis,
    GrowthDto Growth,
    IReadOnlyList<TrendPoint> Trend,
    IReadOnlyList<RankRow> TopTourTypes,
    IReadOnlyList<BranchSplitRow> ByBranch);

public class DashboardQueryHandler : IRequestHandler<DashboardQuery, DashboardDto>
{
    private readonly IFinanceDbContext _db;
    public DashboardQueryHandler(IFinanceDbContext db) => _db = db;

    public async Task<DashboardDto> Handle(DashboardQuery q, CancellationToken ct)
    {
        var from = q.From;
        var to = q.To;

        var dailyQuery = _db.RevenueDaily.AsQueryable();
        if (from is not null) dailyQuery = dailyQuery.Where(r => r.Day >= from.Value.Date);
        if (to is not null) dailyQuery = dailyQuery.Where(r => r.Day <= to.Value.Date);
        if (q.BranchId is not null) dailyQuery = dailyQuery.Where(r => r.BranchId == q.BranchId);
        if (q.Currency is not null) dailyQuery = dailyQuery.Where(r => r.Currency == q.Currency);
        var daily = await dailyQuery.ToListAsync(ct);

        var snapQuery = _db.BookingFinancialSnapshots.AsQueryable();
        if (from is not null) snapQuery = snapQuery.Where(s => s.BookingDateUtc >= from);
        if (to is not null) snapQuery = snapQuery.Where(s => s.BookingDateUtc <= to);
        if (q.BranchId is not null) snapQuery = snapQuery.Where(s => s.BranchId == q.BranchId);
        if (q.Currency is not null) snapQuery = snapQuery.Where(s => s.Currency == q.Currency);
        var snaps = await snapQuery.ToListAsync(ct);

        var active = snaps.Where(s => s.Status != "Cancelled").ToList();

        var recognized = daily.Sum(d => d.Recognized);
        var collected = daily.Sum(d => d.Collected);
        var supplierCost = daily.Sum(d => d.SupplierCost);
        var refunds = daily.Sum(d => d.Refunds);
        var netProfit = recognized - supplierCost - refunds;
        var grossMargin = recognized == 0 ? 0m : Math.Round((recognized - supplierCost) / recognized * 100m, 2);
        var outstandingAr = active.Sum(s => s.Due);
        var bookings = active.Count;
        var avgValue = bookings == 0 ? 0m : Math.Round(active.Sum(s => s.Net + s.Tax) / bookings, 2);
        var cancelled = snaps.Count(s => s.Status == "Cancelled");
        var cancellationRate = snaps.Count == 0 ? 0m : Math.Round((decimal)cancelled / snaps.Count * 100m, 2);
        var refundRate = recognized == 0 ? 0m : Math.Round(refunds / recognized * 100m, 2);

        var kpis = new KpiCards(recognized, collected, supplierCost, netProfit, grossMargin,
            outstandingAr, refunds, bookings, avgValue, cancellationRate, refundRate);

        var trend = BuildTrend(daily, q.Granularity);
        var growth = BuildGrowth(trend);

        var topTourTypes = active
            .GroupBy(s => s.TourTypeCode ?? "UNSET")
            .Select(g => new RankRow(g.Key, g.Sum(s => s.Net), g.Sum(s => s.Margin), g.Count()))
            .OrderByDescending(r => r.Revenue)
            .Take(10)
            .ToList();

        var byBranch = daily
            .GroupBy(d => d.BranchId)
            .Select(g => new BranchSplitRow(g.Key, g.Sum(d => d.Recognized), g.Sum(d => d.SupplierCost),
                g.Sum(d => d.Margin)))
            .OrderByDescending(r => r.Revenue)
            .ToList();

        return new DashboardDto(kpis, growth, trend, topTourTypes, byBranch);
    }

    private static List<TrendPoint> BuildTrend(List<Domain.Entities.RevenueDaily> daily, string granularity)
    {
        Func<DateTime, string> key = granularity.ToLowerInvariant() switch
        {
            "month" => d => d.ToString("yyyy-MM"),
            "week" => d => System.Globalization.ISOWeek.GetYear(d) + "-W" +
                           System.Globalization.ISOWeek.GetWeekOfYear(d).ToString("00"),
            "quarter" => d => d.Year + "-Q" + ((d.Month - 1) / 3 + 1),
            _ => d => d.ToString("yyyy-MM-dd")
        };

        return daily
            .GroupBy(d => key(d.Day))
            .Select(g => new TrendPoint(g.Key, g.Sum(x => x.Recognized), g.Sum(x => x.Collected),
                g.Sum(x => x.Margin), g.Sum(x => x.Refunds)))
            .OrderBy(p => p.Period)
            .ToList();
    }

    private static GrowthDto BuildGrowth(List<TrendPoint> trend)
    {
        if (trend.Count == 0) return new GrowthDto(0m, 0m, 0m);
        var current = trend[^1].Recognized;
        var previous = trend.Count >= 2 ? trend[^2].Recognized : 0m;
        var pct = previous == 0 ? 0m : Math.Round((current - previous) / previous * 100m, 2);
        return new GrowthDto(pct, previous, current);
    }
}
