using MassTransit;
using Microsoft.EntityFrameworkCore;
using Seadora.Booking.Application.Common.Interfaces;
using Seadora.Common.Messaging.Idempotency;
using Seadora.Contracts.Events;

namespace Seadora.Booking.Application.Integration;

// ponytail: UpdateBookingPayment stays live until admin is rewired (Task 3.8/3.9) - this consumer is the
// forward path: Finance owns the payment ledger and Booking just mirrors the cumulative figure.
public sealed class PaymentRecordedConsumer : IConsumer<PaymentRecorded>
{
    public const string ConsumerName = nameof(PaymentRecordedConsumer);

    private readonly IBookingDbContext _context;
    private readonly IIdempotentConsumer _idem;

    public PaymentRecordedConsumer(IBookingDbContext context, IIdempotentConsumer idem)
    {
        _context = context;
        _idem = idem;
    }

    public Task Consume(ConsumeContext<PaymentRecorded> ctx) => HandleAsync(ctx.Message, ctx.CancellationToken);

    public async Task HandleAsync(PaymentRecorded evt, CancellationToken ct = default)
    {
        if (await _idem.AlreadyProcessed(evt.Id, ConsumerName, ct)) return;

        var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == evt.BookingId, ct);
        if (booking is null)
        {
            // tolerate a payment for a booking this service doesn't know about - retrying won't conjure it.
            await _idem.MarkProcessed(evt.Id, ConsumerName, ct);
            return;
        }

        var total = booking.Money?.Total ?? booking.TotalPrice;
        var paid = Math.Min(evt.CumulativePaid, total);
        if (booking.Money is not null) booking.Money = booking.Money.WithPayment(paid);
        booking.IsPaid = evt.CumulativePaid >= total;

        await _context.SaveChangesAsync(ct);
        await _idem.MarkProcessed(evt.Id, ConsumerName, ct);
    }
}
