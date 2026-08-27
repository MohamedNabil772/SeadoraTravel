using MassTransit;
using Microsoft.EntityFrameworkCore;
using Seadora.Common.Messaging.Idempotency;
using Seadora.Contracts.Events;
using Seadora.Customer.Application.Common.Interfaces;
using Seadora.Customer.Domain.Entities;

namespace Seadora.Customer.Application.Integration;

// ponytail: depends on ICustomerDbContext, not the concrete context - Infrastructure references
// Application, so the concrete type would be a circular project reference.
public sealed class BookingPlacedConsumer : IConsumer<BookingPlaced>
{
    public const string Consumer = nameof(BookingPlacedConsumer) + ":" + nameof(BookingPlaced);

    private readonly ICustomerDbContext _db;
    private readonly IIdempotentConsumer _idem;

    public BookingPlacedConsumer(ICustomerDbContext db, IIdempotentConsumer idem)
    {
        _db = db;
        _idem = idem;
    }

    public Task Consume(ConsumeContext<BookingPlaced> ctx) => HandleAsync(ctx.Message, ctx.CancellationToken);

    public async Task HandleAsync(BookingPlaced evt, CancellationToken ct = default)
    {
        if (await _idem.AlreadyProcessed(evt.Id, Consumer, ct)) return;

        var email = Seadora.Customer.Domain.Entities.Customer.NormalizeEmail(evt.CustomerEmail);

        var customer = await _db.Customers
            .FirstOrDefaultAsync(c => c.BranchId == evt.BranchId && c.Email == email, ct);

        if (customer is null)
        {
            customer = new Seadora.Customer.Domain.Entities.Customer
            {
                Id = Guid.NewGuid(),
                BranchId = evt.BranchId,
                FullName = evt.CustomerName,
                Email = email,
                Phone = evt.Phone,
                MarketingConsent = false,
                CreatedUtc = DateTime.UtcNow,
                UpdatedUtc = DateTime.UtcNow
            };
            _db.Customers.Add(customer);
        }

        // ponytail: BookingId is unique, so even a *different* event id for the same booking
        // can't duplicate the history row.
        if (!await _db.BookingHistory.AnyAsync(h => h.BookingId == evt.BookingId, ct))
        {
            _db.BookingHistory.Add(new CustomerBookingHistory
            {
                Id = Guid.NewGuid(),
                CustomerId = customer.Id,
                BookingId = evt.BookingId,
                BranchId = evt.BranchId,
                TourId = evt.TourId,
                TourDate = evt.TourDate,
                Amount = evt.Amount,
                Currency = evt.Currency,
                PlacedUtc = evt.OccurredUtc
            });
        }

        await _db.SaveChangesAsync(ct);
        await _idem.MarkProcessed(evt.Id, Consumer, ct);
    }
}
