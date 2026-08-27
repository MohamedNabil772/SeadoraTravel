using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Common.Messaging.Outbox;
using Seadora.Contracts.Events;
using Seadora.Finance.Application.Common;
using Seadora.Finance.Application.Common.Interfaces;
using Seadora.Finance.Domain.Entities;
using Seadora.Finance.Domain.Enums;
using Seadora.Finance.Domain.Posting;

namespace Seadora.Finance.Application.Payments.Commands.RecordPayment;

/// <summary>
/// Records a customer receipt against a booking: persists the <see cref="Payment"/>, posts a
/// Dr Cash / Cr AR journal entry, updates the booking snapshot (Paid/Due) and the daily collected
/// projection, then emits <c>PaymentRecorded</c> through the transactional outbox so Booking can
/// sync its own paid state.
/// </summary>
public record RecordPaymentCommand(
    Guid BookingId,
    decimal Amount,
    PaymentMethod Method,
    string? Reference,
    DateTime? ReceivedUtc,
    string? CreatedBy) : IRequest<Guid>;

public class RecordPaymentCommandValidator : AbstractValidator<RecordPaymentCommand>
{
    public RecordPaymentCommandValidator()
    {
        RuleFor(x => x.BookingId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0m);
    }
}

public class RecordPaymentCommandHandler : IRequestHandler<RecordPaymentCommand, Guid>
{
    private readonly IFinanceDbContext _db;
    private readonly IOutboxWriter _outbox;

    public RecordPaymentCommandHandler(IFinanceDbContext db, IOutboxWriter outbox)
    {
        _db = db;
        _outbox = outbox;
    }

    public async Task<Guid> Handle(RecordPaymentCommand cmd, CancellationToken ct)
    {
        var snap = await _db.BookingFinancialSnapshots.FirstOrDefaultAsync(s => s.BookingId == cmd.BookingId, ct)
            ?? throw new KeyNotFoundException($"No financial snapshot for booking {cmd.BookingId}; revenue must be recognized before a payment can be recorded.");

        var amount = Math.Round(cmd.Amount, 2, MidpointRounding.AwayFromZero);
        var received = cmd.ReceivedUtc ?? DateTime.UtcNow;

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            BookingId = snap.BookingId,
            BranchId = snap.BranchId,
            CustomerId = snap.CustomerId,
            Amount = amount,
            Currency = snap.Currency,
            Method = cmd.Method,
            Reference = cmd.Reference,
            ReceivedUtc = received,
            CreatedBy = cmd.CreatedBy
        };
        _db.Payments.Add(payment);

        var fx = await FxResolver.ResolveRateAsync(_db, snap.Currency, received, ct);
        _db.JournalEntries.Add(LedgerPosting.PaymentReceipt(
            snap.BookingId, snap.BranchId, amount, snap.Currency, fx, received, payment.Id.ToString()));

        var total = snap.Net + snap.Tax;
        snap.Paid += amount;
        snap.Due = Math.Max(0m, total - snap.Paid);
        snap.UpdatedUtc = DateTime.UtcNow;

        await AddCollectedAsync(snap.BranchId, received.Date, snap.Currency, amount, ct);

        _outbox.Enqueue(new PaymentRecorded
        {
            OccurredUtc = received,
            PaymentId = payment.Id,
            BookingId = snap.BookingId,
            BranchId = snap.BranchId,
            Amount = amount,
            Currency = snap.Currency,
            CumulativePaid = snap.Paid,
            BookingTotal = total,
            Method = cmd.Method.ToString(),
            ReceivedUtc = received
        });

        await _db.SaveChangesAsync(ct);
        return payment.Id;
    }

    private async Task AddCollectedAsync(Guid branchId, DateTime day, string currency, decimal amount, CancellationToken ct)
    {
        var row = await _db.RevenueDaily.FirstOrDefaultAsync(
            r => r.BranchId == branchId && r.Day == day && r.Currency == currency, ct);
        if (row is null)
        {
            row = new RevenueDaily
            {
                Id = Guid.NewGuid(),
                BranchId = branchId,
                Day = day,
                Currency = currency
            };
            _db.RevenueDaily.Add(row);
        }
        row.Collected += amount;
    }
}
