using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Seadora.Booking.Application.Bookings.Commands.CreateBooking;
using Seadora.Booking.Application.Bookings.Commands.UpdateBookingPayment;
using Seadora.Booking.Application.Common.Interfaces;
using Seadora.Booking.Domain.Entities;
using Seadora.Booking.Infrastructure.Persistence;
using Seadora.Common.Tenancy;
using Seadora.Contracts.Enums;

namespace Seadora.UnitTests;

// ponytail: same InMemory harness as AllocationTests - the jsonb collections can't be mapped, and
// Money is computed from the in-memory SelectedAddons before insert, so ignoring them is fine.
file sealed class MoneyTestDbContext(DbContextOptions<BookingDbContext> options) : BookingDbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Seadora.Booking.Domain.Entities.Booking>().Ignore(b => b.GuestsList);
        modelBuilder.Entity<Seadora.Booking.Domain.Entities.Booking>().Ignore(b => b.SelectedAddons);
    }
}

file sealed class NoOpWhatsApp : IWhatsAppNotificationService
{
    public Task<bool> SendBookingConfirmationAsync(Seadora.Booking.Domain.Entities.Booking booking, CancellationToken cancellationToken = default) => Task.FromResult(true);
    public Task<bool> SendCustomMessageAsync(string toWhatsApp, string message, CancellationToken cancellationToken = default) => Task.FromResult(true);
}

file sealed class NoOpEmail : IEmailSender
{
    public Task SendAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task SendTemplatedAsync(string to, string subject, string templateHtml, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task SendEmailAsync(string to, string subject, string htmlMessage, CancellationToken cancellationToken = default) => Task.CompletedTask;
}

file sealed class HeadOfficeBranch : ICurrentBranch
{
    public Guid BranchId => SeadoraBranches.HeadOffice;
}

public class BookingMoneyTests
{
    private static readonly DateTime TourDay = new(2026, 9, 1, 0, 0, 0, DateTimeKind.Utc);

    private static (CreateBookingCommandHandler Handler, BookingDbContext Db) Build()
    {
        var options = new DbContextOptionsBuilder<BookingDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        BookingDbContext db = new MoneyTestDbContext(options);
        var handler = new CreateBookingCommandHandler(
            db, new NoOpWhatsApp(), new NoOpEmail(), new HeadOfficeBranch(),
            NullLogger<CreateBookingCommandHandler>.Instance);
        return (handler, db);
    }

    private static Guid SeedProjection(BookingDbContext db)
    {
        var tourId = Guid.NewGuid();
        db.TourProjections.Add(new TourProjection
        {
            TourId = tourId,
            BranchId = SeadoraBranches.HeadOffice,
            TourTypeCode = "PRIVATE_YACHT",
            AllocationModel = AllocationModel.Shared,
            MinCapacity = 1,
            MaxCapacity = 20,
            Currency = "USD",
            UpdatedUtc = DateTime.UtcNow
        });
        db.SaveChanges();
        db.ChangeTracker.Clear();
        return tourId;
    }

    private static CreateBookingCommand Command(Guid tourId, decimal totalPrice, decimal addonUnitPrice) => new(
        TourId: tourId,
        CustomerName: "Test Guest",
        CustomerEmail: "guest@example.com",
        TourDate: TourDay,
        Guests: 2,
        PassportFileName: "passport.pdf",
        TotalPrice: totalPrice,
        SelectedAddons: new List<BookingAddonSnapshot>
        {
            new() { AddonId = Guid.NewGuid(), Title = "Lunch", UnitPrice = addonUnitPrice, Quantity = 1 }
        });

    [Fact]
    public async Task Create_Populates_Money_Breakdown_And_Snapshot()
    {
        var (handler, db) = Build();
        var tourId = SeedProjection(db);

        var id = await handler.Handle(Command(tourId, totalPrice: 500m, addonUnitPrice: 80m), default);

        db.ChangeTracker.Clear();
        var booking = await db.Bookings.SingleAsync(b => b.Id == id);

        Assert.NotNull(booking.Money);
        Assert.Equal(80m, booking.Money!.AddonsTotal);
        Assert.Equal(420m, booking.Money.Subtotal);
        Assert.Equal(500m, booking.Money.Total);
        Assert.Equal(500m, booking.Money.BalanceDue);
        Assert.Equal(0m, booking.Money.AmountPaid);
        Assert.Equal("USD", booking.Money.Currency);

        Assert.Equal(500m, booking.TotalPrice);
        Assert.Equal("PRIVATE_YACHT", booking.TourTypeCode);
        Assert.Equal(SeadoraBranches.HeadOffice, booking.BranchId);
        Assert.Null(booking.CustomerId);
    }

    [Fact]
    public async Task Create_Without_Projection_Defaults_Currency_And_Null_TourType()
    {
        var (handler, db) = Build();

        var id = await handler.Handle(Command(Guid.NewGuid(), totalPrice: 100m, addonUnitPrice: 0m), default);

        db.ChangeTracker.Clear();
        var booking = await db.Bookings.SingleAsync(b => b.Id == id);

        Assert.Equal("EUR", booking.Money!.Currency);
        Assert.Equal(100m, booking.Money.Total);
        Assert.Null(booking.TourTypeCode);
    }

    [Fact]
    public async Task UpdatePayment_Syncs_Money_With_IsPaid()
    {
        var (handler, db) = Build();
        var tourId = SeedProjection(db);
        var id = await handler.Handle(Command(tourId, totalPrice: 500m, addonUnitPrice: 80m), default);
        db.ChangeTracker.Clear();

        var payment = new UpdateBookingPaymentCommandHandler(db);
        await payment.Handle(new UpdateBookingPaymentCommand(id, true), default);

        db.ChangeTracker.Clear();
        var paid = await db.Bookings.SingleAsync(b => b.Id == id);
        Assert.True(paid.IsPaid);
        Assert.Equal(500m, paid.Money!.AmountPaid);
        Assert.Equal(0m, paid.Money.BalanceDue);

        await payment.Handle(new UpdateBookingPaymentCommand(id, false), default);

        db.ChangeTracker.Clear();
        var unpaid = await db.Bookings.SingleAsync(b => b.Id == id);
        Assert.False(unpaid.IsPaid);
        Assert.Equal(0m, unpaid.Money!.AmountPaid);
        Assert.Equal(500m, unpaid.Money.BalanceDue);
    }
}
