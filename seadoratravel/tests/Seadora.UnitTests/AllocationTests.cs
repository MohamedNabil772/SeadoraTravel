using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Seadora.Booking.Application.Bookings.Commands.CreateBooking;
using Seadora.Booking.Application.Bookings.Queries;
using Seadora.Booking.Application.Common.Interfaces;
using Seadora.Booking.Domain.Entities;
using Seadora.Booking.Infrastructure.Persistence;
using Seadora.Common.Tenancy;
using Seadora.Contracts.Enums;

namespace Seadora.UnitTests;

// ponytail: InMemory can't map the Booking jsonb collections; ignore them, capacity logic doesn't read them.
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

public class AllocationTests
{
    private static readonly DateTime TourDay = new(2026, 9, 1, 0, 0, 0, DateTimeKind.Utc);

    private static (CreateBookingCommandHandler Handler, BookingDbContext Db) Build()
    {
        var options = new DbContextOptionsBuilder<BookingDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        BookingDbContext db = new TestBookingDbContext(options);
        var handler = new CreateBookingCommandHandler(
            db, new NoOpWhatsApp(), new NoOpEmail(), new HeadOfficeBranch(),
            new Seadora.Common.Messaging.Outbox.OutboxWriter(db),
            NullLogger<CreateBookingCommandHandler>.Instance);
        return (handler, db);
    }

    private static void SeedProjection(BookingDbContext db, Guid tourId, int maxCapacity, AllocationModel model)
    {
        db.TourProjections.Add(new TourProjection
        {
            TourId = tourId,
            BranchId = SeadoraBranches.HeadOffice,
            TourTypeCode = "GROUP",
            AllocationModel = model,
            MinCapacity = 1,
            MaxCapacity = maxCapacity,
            Currency = "EUR",
            UpdatedUtc = DateTime.UtcNow
        });
        db.SaveChanges();
        db.ChangeTracker.Clear();
    }

    private static CreateBookingCommand Booking(Guid tourId, int guests) => new(
        TourId: tourId,
        CustomerName: "Test Guest",
        CustomerEmail: "guest@example.com",
        TourDate: TourDay,
        Guests: guests,
        PassportFileName: "passport.pdf");

    [Fact]
    public async Task Shared_Rejects_Booking_That_Exceeds_Capacity()
    {
        var (handler, db) = Build();
        var tourId = Guid.NewGuid();
        SeedProjection(db, tourId, maxCapacity: 2, AllocationModel.Shared);

        await handler.Handle(Booking(tourId, 2), default);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.Handle(Booking(tourId, 1), default));
        Assert.Contains("Not enough capacity", ex.Message);

        db.ChangeTracker.Clear();
        Assert.Equal(1, await db.Bookings.CountAsync());
        Assert.Equal(2, await db.Bookings.SumAsync(b => b.Guests));
    }

    [Fact]
    public async Task WholeUnit_Rejects_Second_Booking_On_Same_Departure()
    {
        var (handler, db) = Build();
        var tourId = Guid.NewGuid();
        SeedProjection(db, tourId, maxCapacity: 20, AllocationModel.WholeUnit);

        await handler.Handle(Booking(tourId, 1), default);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.Handle(Booking(tourId, 1), default));
        Assert.Contains("whole-unit", ex.Message);

        db.ChangeTracker.Clear();
        Assert.Equal(1, await db.Bookings.CountAsync());
    }

    [Fact]
    public async Task Unknown_Tour_Without_Projection_Is_Unbounded()
    {
        var (handler, db) = Build();
        var tourId = Guid.NewGuid();

        await handler.Handle(Booking(tourId, 50), default);

        db.ChangeTracker.Clear();
        var departure = Assert.Single(db.Departures);
        Assert.Equal(int.MaxValue, departure.Capacity);
        Assert.Equal(AllocationModel.Shared, departure.AllocationModel);
        Assert.Equal(SeadoraBranches.HeadOffice, departure.BranchId);
    }

    [Fact]
    public async Task CreateBooking_Enqueues_One_BookingPlaced_Outbox_Row()
    {
        var (handler, db) = Build();
        var tourId = Guid.NewGuid();
        SeedProjection(db, tourId, maxCapacity: 10, AllocationModel.Shared);

        var bookingId = await handler.Handle(Booking(tourId, 2), default);

        db.ChangeTracker.Clear();
        var row = Assert.Single(db.OutboxMessages);
        var evt = System.Text.Json.JsonSerializer.Deserialize<Seadora.Contracts.Events.BookingPlaced>(row.Payload);
        Assert.NotNull(evt);
        Assert.Equal(bookingId, evt!.BookingId);
    }

    [Fact]
    public async Task Availability_Returns_Remaining_Capacity()
    {
        var (_, db) = Build();
        var tourId = Guid.NewGuid();
        db.Departures.Add(new Departure
        {
            Id = Guid.NewGuid(),
            BranchId = SeadoraBranches.HeadOffice,
            TourId = tourId,
            StartUtc = TourDay,
            TimeSlot = "",
            Capacity = 5,
            AllocationModel = AllocationModel.Shared
        });
        db.Bookings.Add(new Seadora.Booking.Domain.Entities.Booking
        {
            Id = Guid.NewGuid(),
            TourId = tourId,
            CustomerName = "Test Guest",
            CustomerEmail = "guest@example.com",
            TourDate = TourDay,
            Guests = 2,
            BookingDate = DateTime.UtcNow,
            Status = Seadora.Booking.Domain.Enums.BookingStatus.Pending
        });
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();

        var remaining = await new GetTourAvailabilityQueryHandler(db)
            .Handle(new GetTourAvailabilityQuery(tourId, TourDay), default);

        Assert.Equal(3, remaining);
    }

    [Fact]
    public async Task CommitWithRetry_Retries_After_Concurrency_Conflict()
    {
        var attempts = 0;
        var resets = 0;

        await CreateBookingCommandHandler.CommitWithRetryAsync(() =>
        {
            attempts++;
            if (attempts == 1) throw new DbUpdateConcurrencyException("conflict");
            return Task.CompletedTask;
        }, () => resets++);

        Assert.Equal(2, attempts);
        Assert.Equal(1, resets);
    }

    [Fact]
    public async Task CommitWithRetry_Gives_Up_After_Max_Attempts()
    {
        var attempts = 0;

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            CreateBookingCommandHandler.CommitWithRetryAsync(() =>
            {
                attempts++;
                throw new DbUpdateConcurrencyException("conflict");
            }, () => { }));

        Assert.Equal(3, attempts);
        Assert.Contains("high demand", ex.Message);
    }

    [Fact]
    public async Task CommitWithRetry_Does_Not_Retry_Capacity_Rejections()
    {
        var attempts = 0;

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            CreateBookingCommandHandler.CommitWithRetryAsync(() =>
            {
                attempts++;
                throw new InvalidOperationException("Not enough capacity");
            }, () => { }));

        Assert.Equal(1, attempts);
    }
}
