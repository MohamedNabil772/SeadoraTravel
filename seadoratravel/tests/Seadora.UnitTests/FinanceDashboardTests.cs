using Microsoft.EntityFrameworkCore;
using Seadora.Finance.Application.Dashboard;
using Seadora.Finance.Domain.Entities;
using Seadora.Finance.Infrastructure.Persistence;

namespace Seadora.UnitTests;

public class FinanceDashboardTests
{
    private static FinanceDbContext NewDb() =>
        new(new DbContextOptionsBuilder<FinanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    private static BookingFinancialSnapshot Snap(Guid branch, decimal net, decimal tax, decimal supplierCost,
        decimal due, string status = "Recognized", string? tourType = "GROUP", DateTime? date = null)
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
            Currency = "USD",
            Status = status,
            BookingDateUtc = date ?? DateTime.UtcNow,
            UpdatedUtc = DateTime.UtcNow
        };

    private static RevenueDaily Daily(Guid branch, DateTime day, decimal recognized, decimal collected,
        decimal supplierCost, decimal margin, decimal refunds = 0m)
        => new()
        {
            Id = Guid.NewGuid(),
            BranchId = branch,
            Day = day,
            Currency = "USD",
            Recognized = recognized,
            Collected = collected,
            SupplierCost = supplierCost,
            Margin = margin,
            Refunds = refunds
        };

    [Fact]
    public async Task Kpis_Compute_Profit_Margin_And_Ar()
    {
        var db = NewDb();
        var branch = Guid.NewGuid();
        db.RevenueDaily.Add(Daily(branch, new DateTime(2026, 1, 1), recognized: 1000m, collected: 700m, supplierCost: 400m, margin: 600m, refunds: 50m));
        db.BookingFinancialSnapshots.Add(Snap(branch, net: 1000m, tax: 100m, supplierCost: 400m, due: 300m));
        await db.SaveChangesAsync();

        var dash = await new DashboardQueryHandler(db).Handle(new DashboardQuery(), default);

        Assert.Equal(1000m, dash.Kpis.RevenueRecognized);
        Assert.Equal(700m, dash.Kpis.RevenueCollected);
        Assert.Equal(550m, dash.Kpis.NetProfit);      // 1000 - 400 - 50
        Assert.Equal(60m, dash.Kpis.GrossMarginPct);  // (1000-400)/1000
        Assert.Equal(300m, dash.Kpis.OutstandingAr);
        Assert.Equal(1, dash.Kpis.Bookings);
    }

    [Fact]
    public async Task CancellationRate_Excludes_Cancelled_From_Active()
    {
        var db = NewDb();
        var branch = Guid.NewGuid();
        db.BookingFinancialSnapshots.Add(Snap(branch, 100m, 0m, 0m, due: 0m));
        db.BookingFinancialSnapshots.Add(Snap(branch, 100m, 0m, 0m, due: 0m));
        db.BookingFinancialSnapshots.Add(Snap(branch, 100m, 0m, 0m, due: 0m, status: "Cancelled"));
        await db.SaveChangesAsync();

        var dash = await new DashboardQueryHandler(db).Handle(new DashboardQuery(), default);

        Assert.Equal(2, dash.Kpis.Bookings);
        Assert.Equal(33.33m, dash.Kpis.CancellationRatePct);
    }

    [Fact]
    public async Task Trend_Groups_By_Month_And_Computes_Growth()
    {
        var db = NewDb();
        var branch = Guid.NewGuid();
        db.RevenueDaily.Add(Daily(branch, new DateTime(2026, 1, 15), 100m, 100m, 0m, 100m));
        db.RevenueDaily.Add(Daily(branch, new DateTime(2026, 2, 10), 150m, 150m, 0m, 150m));
        db.RevenueDaily.Add(Daily(branch, new DateTime(2026, 2, 20), 50m, 50m, 0m, 50m));
        await db.SaveChangesAsync();

        var dash = await new DashboardQueryHandler(db).Handle(new DashboardQuery(Granularity: "month"), default);

        Assert.Equal(2, dash.Trend.Count);
        Assert.Equal("2026-01", dash.Trend[0].Period);
        Assert.Equal(200m, dash.Trend[1].Recognized); // Feb 150+50
        Assert.Equal(100m, dash.Growth.RevenueMoMPct); // (200-100)/100
    }

    [Fact]
    public async Task TopTourTypes_Ranked_By_Revenue()
    {
        var db = NewDb();
        var branch = Guid.NewGuid();
        db.BookingFinancialSnapshots.Add(Snap(branch, 250m, 0m, 100m, due: 0m, tourType: "VIP"));
        db.BookingFinancialSnapshots.Add(Snap(branch, 100m, 0m, 20m, due: 0m, tourType: "GROUP"));
        db.BookingFinancialSnapshots.Add(Snap(branch, 200m, 0m, 50m, due: 0m, tourType: "GROUP"));
        await db.SaveChangesAsync();

        var dash = await new DashboardQueryHandler(db).Handle(new DashboardQuery(), default);

        Assert.Equal("GROUP", dash.TopTourTypes[0].Key); // 100+200=300 net
        Assert.Equal(300m, dash.TopTourTypes[0].Revenue);
        Assert.Equal(2, dash.TopTourTypes[0].Bookings);
    }
}
