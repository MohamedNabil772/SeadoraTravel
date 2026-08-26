using Microsoft.EntityFrameworkCore;
using Seadora.Booking.Application.Integration;
using Seadora.Booking.Infrastructure.Persistence;
using Seadora.Common.Messaging.Idempotency;
using Seadora.Contracts.Enums;
using Seadora.Contracts.Events;

namespace Seadora.UnitTests;

// ponytail: InMemory can't map the Booking jsonb collections; ignore them, this suite only touches projections.
file sealed class TestBookingDbContext(DbContextOptions<BookingDbContext> options) : BookingDbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Seadora.Booking.Domain.Entities.Booking>().Ignore(b => b.GuestsList);
        modelBuilder.Entity<Seadora.Booking.Domain.Entities.Booking>().Ignore(b => b.SelectedAddons);
    }
}

public class TourProjectionConsumerTests
{
    private static (TourProjectionConsumers Consumers, BookingDbContext Db) Build()
    {
        var options = new DbContextOptionsBuilder<BookingDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        BookingDbContext db = new TestBookingDbContext(options);
        return (new TourProjectionConsumers(db, new IdempotentConsumer(db)), db);
    }

    private static TourPublished Sample(Guid tourId) => new()
    {
        TourId = tourId,
        BranchId = Guid.NewGuid(),
        TourTypeCode = "GROUP",
        AllocationModel = AllocationModel.Shared,
        MinCapacity = 2,
        MaxCapacity = 12,
        RequiresGuestDetails = true,
        RequiresPassport = false,
        PayLaterAllowed = true,
        PriceFrom = 99.5m,
        Currency = "EUR"
    };

    [Fact]
    public async Task TourUpsert_Inserts_Projection_From_Event()
    {
        var (consumers, db) = Build();
        var evt = Sample(Guid.NewGuid());

        await consumers.HandleTourUpsertAsync(evt);

        var row = Assert.Single(db.TourProjections);
        Assert.Equal(evt.TourId, row.TourId);
        Assert.Equal(evt.BranchId, row.BranchId);
        Assert.Equal("GROUP", row.TourTypeCode);
        Assert.Equal(AllocationModel.Shared, row.AllocationModel);
        Assert.Equal(2, row.MinCapacity);
        Assert.Equal(12, row.MaxCapacity);
        Assert.True(row.RequiresGuestDetails);
        Assert.False(row.RequiresPassport);
        Assert.True(row.PayLaterAllowed);
        Assert.Equal(99.5m, row.PriceFrom);
        Assert.Equal("EUR", row.Currency);
    }

    [Fact]
    public async Task TourUpsert_Is_Idempotent_On_Same_EventId()
    {
        var (consumers, db) = Build();
        var evt = Sample(Guid.NewGuid());
        await consumers.HandleTourUpsertAsync(evt);

        var redelivery = evt with { MaxCapacity = 999, PriceFrom = 1m };
        await consumers.HandleTourUpsertAsync(redelivery);

        var row = Assert.Single(db.TourProjections);
        Assert.Equal(12, row.MaxCapacity);
        Assert.Equal(99.5m, row.PriceFrom);
    }

    [Fact]
    public async Task PolicyChange_Updates_Policy_Fields_Only()
    {
        var (consumers, db) = Build();
        var evt = Sample(Guid.NewGuid());
        await consumers.HandleTourUpsertAsync(evt);

        await consumers.HandlePolicyChangeAsync(new TourTypePolicyChanged
        {
            Code = "GROUP",
            AllocationModel = AllocationModel.WholeUnit,
            RequiresGuestDetails = false,
            RequiresPassport = true,
            PayLaterAllowed = false
        });

        var row = Assert.Single(db.TourProjections);
        Assert.Equal(AllocationModel.WholeUnit, row.AllocationModel);
        Assert.False(row.RequiresGuestDetails);
        Assert.True(row.RequiresPassport);
        Assert.False(row.PayLaterAllowed);
        Assert.Equal(2, row.MinCapacity);
        Assert.Equal(12, row.MaxCapacity);
        Assert.Equal(99.5m, row.PriceFrom);
    }
}
