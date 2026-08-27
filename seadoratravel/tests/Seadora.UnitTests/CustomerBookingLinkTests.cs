using Microsoft.EntityFrameworkCore;
using Seadora.Common.Messaging.Idempotency;
using Seadora.Contracts.Events;
using Seadora.Customer.Application.Integration;
using Seadora.Customer.Infrastructure.Persistence;

namespace Seadora.UnitTests;

public class CustomerBookingLinkTests
{
    private static readonly Guid Branch = Guid.Parse("00000000-0000-0000-0000-0000000000aa");

    private static (BookingPlacedConsumer Consumer, CustomerDbContext Db) Build()
    {
        var db = new CustomerDbContext(new DbContextOptionsBuilder<CustomerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);
        return (new BookingPlacedConsumer(db, new IdempotentConsumer(db)), db);
    }

    private static BookingPlaced Event(string email = "Ada@Example.COM", Guid? bookingId = null) => new()
    {
        BookingId = bookingId ?? Guid.NewGuid(),
        BranchId = Branch,
        CustomerEmail = email,
        CustomerName = "Ada Lovelace",
        Phone = "+201234567890",
        TourId = Guid.NewGuid(),
        TourDate = new DateTime(2026, 9, 1, 0, 0, 0, DateTimeKind.Utc),
        Amount = 250m,
        Currency = "EUR"
    };

    [Fact]
    public async Task Unknown_Customer_Is_Created_With_Normalized_Email_And_History()
    {
        var (consumer, db) = Build();
        var evt = Event();

        await consumer.HandleAsync(evt);

        var customer = Assert.Single(db.Customers);
        Assert.Equal("ada@example.com", customer.Email);
        Assert.Equal(Branch, customer.BranchId);
        Assert.Equal("Ada Lovelace", customer.FullName);

        var history = Assert.Single(db.BookingHistory);
        Assert.Equal(customer.Id, history.CustomerId);
        Assert.Equal(evt.BookingId, history.BookingId);
        Assert.Equal(250m, history.Amount);
        Assert.Equal(evt.OccurredUtc, history.PlacedUtc);
    }

    [Fact]
    public async Task Existing_Customer_Is_Reused_Not_Duplicated()
    {
        var (consumer, db) = Build();
        db.Customers.Add(new Seadora.Customer.Domain.Entities.Customer
        {
            Id = Guid.NewGuid(),
            BranchId = Branch,
            FullName = "Ada",
            Email = "ada@example.com",
            CreatedUtc = DateTime.UtcNow,
            UpdatedUtc = DateTime.UtcNow
        });
        await db.SaveChangesAsync(default);
        var existingId = db.Customers.Single().Id;

        await consumer.HandleAsync(Event("  ADA@Example.com  "));

        Assert.Equal(1, await db.Customers.CountAsync());
        Assert.Equal(existingId, Assert.Single(db.BookingHistory).CustomerId);
    }

    [Fact]
    public async Task Same_Event_Twice_Yields_One_Customer_And_One_History_Row()
    {
        var (consumer, db) = Build();
        var evt = Event();

        await consumer.HandleAsync(evt);
        await consumer.HandleAsync(evt);

        Assert.Equal(1, await db.Customers.CountAsync());
        Assert.Equal(1, await db.BookingHistory.CountAsync());
    }

    [Fact]
    public async Task Second_Booking_Adds_Second_History_Row_Under_Same_Customer()
    {
        var (consumer, db) = Build();

        await consumer.HandleAsync(Event());
        await consumer.HandleAsync(Event());

        var customer = Assert.Single(db.Customers);
        var rows = await db.BookingHistory.ToListAsync();
        Assert.Equal(2, rows.Count);
        Assert.All(rows, r => Assert.Equal(customer.Id, r.CustomerId));
    }
}
