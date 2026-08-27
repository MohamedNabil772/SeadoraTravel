using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Seadora.Booking.Application.Bookings.Commands.CreateBooking;
using Seadora.Booking.Application.Bookings.Commands.UpdateBookingStatus;
using Seadora.Booking.Application.Common.Interfaces;
using Seadora.Booking.Application.Integration;
using Seadora.Booking.Domain.Enums;
using Seadora.Booking.Domain.ValueObjects;
using Seadora.Booking.Infrastructure.Persistence;
using Seadora.Common.Messaging.Idempotency;
using Seadora.Common.Messaging.Outbox;
using Seadora.Common.Tenancy;
using Seadora.Contracts.Events;

namespace Seadora.UnitTests;

// ponytail: InMemory can't map the Booking jsonb collections; ignore them, none of this suite reads them.
file sealed class TestBookingDbContext(DbContextOptions<BookingDbContext> options) : BookingDbContext(options)
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

public class FinanceEventsTests
{
    private static readonly DateTime TourDay = new(2026, 9, 1, 0, 0, 0, DateTimeKind.Utc);

    private static BookingDbContext NewDb()
    {
        var options = new DbContextOptionsBuilder<BookingDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new TestBookingDbContext(options);
    }

    private static T? Payload<T>(BookingDbContext db) where T : class =>
        db.OutboxMessages
            .AsEnumerable()
            .Where(m => m.Type.StartsWith(typeof(T).FullName!, StringComparison.Ordinal))
            .Select(m => JsonSerializer.Deserialize<T>(m.Payload))
            .FirstOrDefault();

    [Fact]
    public async Task CreateBooking_Enqueues_BookingRevenueRecognized()
    {
        var db = NewDb();
        var handler = new CreateBookingCommandHandler(
            db, new NoOpWhatsApp(), new NoOpEmail(), new HeadOfficeBranch(),
            new OutboxWriter(db), NullLogger<CreateBookingCommandHandler>.Instance);

        var bookingId = await handler.Handle(new CreateBookingCommand(
            TourId: Guid.NewGuid(),
            CustomerName: "Test Guest",
            CustomerEmail: "guest@example.com",
            TourDate: TourDay,
            Guests: 2,
            TotalPrice: 250m,
            PassportFileName: "passport.pdf"), default);

        db.ChangeTracker.Clear();
        Assert.Equal(2, db.OutboxMessages.Count());

        var revenue = Payload<BookingRevenueRecognized>(db);
        Assert.NotNull(revenue);
        Assert.Equal(bookingId, revenue!.BookingId);
        Assert.Equal(SeadoraBranches.HeadOffice, revenue.BranchId);
        Assert.Equal(250m, revenue.Total);
        Assert.Equal(250m, revenue.Subtotal);
        Assert.Equal("EUR", revenue.Currency);
        Assert.Null(revenue.SupplierId);
        Assert.Equal(0m, revenue.SupplierPercentage);
        Assert.NotNull(Payload<BookingPlaced>(db));
    }

    [Fact]
    public async Task Cancelling_Booking_Enqueues_One_BookingCancelled()
    {
        var db = NewDb();
        var booking = Seed(db, total: 120m, isPaid: true);

        var handler = new UpdateBookingStatusCommandHandler(
            db, new NoOpWhatsApp(), new NoOpEmail(),
            NullLogger<UpdateBookingStatusCommandHandler>.Instance, new OutboxWriter(db));

        await handler.Handle(new UpdateBookingStatusCommand(booking.Id, BookingStatus.Cancelled), default);

        db.ChangeTracker.Clear();
        var row = Assert.Single(db.OutboxMessages);
        var evt = JsonSerializer.Deserialize<BookingCancelled>(row.Payload);
        Assert.NotNull(evt);
        Assert.Equal(booking.Id, evt!.BookingId);
        Assert.Equal(SeadoraBranches.HeadOffice, evt.BranchId);
        Assert.Equal(0m, evt.RefundAmount);
        Assert.Equal("EUR", evt.Currency);
        Assert.Equal(BookingStatus.Cancelled, db.Bookings.Single().Status);
    }

    [Fact]
    public async Task Confirming_Booking_Enqueues_Nothing()
    {
        var db = NewDb();
        var booking = Seed(db, total: 120m, isPaid: true);

        var handler = new UpdateBookingStatusCommandHandler(
            db, new NoOpWhatsApp(), new NoOpEmail(),
            NullLogger<UpdateBookingStatusCommandHandler>.Instance, new OutboxWriter(db));

        await handler.Handle(new UpdateBookingStatusCommand(booking.Id, BookingStatus.Confirmed), default);

        db.ChangeTracker.Clear();
        Assert.Empty(db.OutboxMessages);
    }

    [Fact]
    public async Task PaymentRecorded_Full_Payment_Marks_Booking_Paid()
    {
        var db = NewDb();
        var booking = Seed(db, total: 100m, isPaid: false);
        var consumer = new PaymentRecordedConsumer(db, new IdempotentConsumer(db));

        await consumer.HandleAsync(Payment(booking.Id, cumulative: 100m, total: 100m));

        db.ChangeTracker.Clear();
        var row = db.Bookings.Single();
        Assert.True(row.IsPaid);
        Assert.Equal(100m, row.Money!.AmountPaid);
        Assert.Equal(0m, row.Money.BalanceDue);
    }

    [Fact]
    public async Task PaymentRecorded_Partial_Payment_Leaves_Balance()
    {
        var db = NewDb();
        var booking = Seed(db, total: 100m, isPaid: false);
        var consumer = new PaymentRecordedConsumer(db, new IdempotentConsumer(db));

        await consumer.HandleAsync(Payment(booking.Id, cumulative: 40m, total: 100m));

        db.ChangeTracker.Clear();
        var row = db.Bookings.Single();
        Assert.False(row.IsPaid);
        Assert.Equal(40m, row.Money!.AmountPaid);
        Assert.Equal(60m, row.Money.BalanceDue);
    }

    [Fact]
    public async Task PaymentRecorded_Redelivery_Is_Idempotent()
    {
        var db = NewDb();
        var booking = Seed(db, total: 100m, isPaid: false);
        var consumer = new PaymentRecordedConsumer(db, new IdempotentConsumer(db));

        var evt = Payment(booking.Id, cumulative: 40m, total: 100m);
        await consumer.HandleAsync(evt);
        await consumer.HandleAsync(evt with { CumulativePaid = 100m });

        db.ChangeTracker.Clear();
        var row = db.Bookings.Single();
        Assert.False(row.IsPaid);
        Assert.Equal(40m, row.Money!.AmountPaid);
    }

    [Fact]
    public async Task PaymentRecorded_Overpayment_Clamps_To_Total()
    {
        var db = NewDb();
        var booking = Seed(db, total: 100m, isPaid: false);
        var consumer = new PaymentRecordedConsumer(db, new IdempotentConsumer(db));

        await consumer.HandleAsync(Payment(booking.Id, cumulative: 130m, total: 100m));

        db.ChangeTracker.Clear();
        var row = db.Bookings.Single();
        Assert.True(row.IsPaid);
        Assert.Equal(100m, row.Money!.AmountPaid);
        Assert.Equal(0m, row.Money.BalanceDue);
    }

    [Fact]
    public async Task PaymentRecorded_Unknown_Booking_Does_Not_Throw_And_Marks_Processed()
    {
        var db = NewDb();
        var consumer = new PaymentRecordedConsumer(db, new IdempotentConsumer(db));
        var evt = Payment(Guid.NewGuid(), cumulative: 50m, total: 100m);

        await consumer.HandleAsync(evt);

        Assert.True(await new IdempotentConsumer(db)
            .AlreadyProcessed(evt.Id, PaymentRecordedConsumer.ConsumerName, default));
    }

    private static PaymentRecorded Payment(Guid bookingId, decimal cumulative, decimal total) => new()
    {
        PaymentId = Guid.NewGuid(),
        BookingId = bookingId,
        BranchId = SeadoraBranches.HeadOffice,
        Amount = cumulative,
        CumulativePaid = cumulative,
        BookingTotal = total,
        ReceivedUtc = DateTime.UtcNow
    };

    private static Seadora.Booking.Domain.Entities.Booking Seed(BookingDbContext db, decimal total, bool isPaid)
    {
        var booking = new Seadora.Booking.Domain.Entities.Booking
        {
            Id = Guid.NewGuid(),
            BranchId = SeadoraBranches.HeadOffice,
            TourId = Guid.NewGuid(),
            CustomerName = "Test Guest",
            CustomerEmail = "guest@example.com",
            TourDate = TourDay,
            Guests = 1,
            TotalPrice = total,
            IsPaid = isPaid,
            MissingIdentification = false,
            BookingDate = DateTime.UtcNow,
            Status = BookingStatus.Pending,
            Money = Money.Create(total, 0m, 0m, 0m, "EUR")
        };
        db.Bookings.Add(booking);
        db.SaveChanges();
        db.ChangeTracker.Clear();
        return booking;
    }
}
