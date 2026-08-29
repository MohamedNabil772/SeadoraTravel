using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Common.Messaging.Outbox;
using Seadora.Contracts.Events;
using Seadora.Finance.Application.Common;
using Seadora.Finance.Application.Common.Interfaces;
using Seadora.Finance.Domain.Entities;
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
    string Method,
    string? Currency,
    decimal? ExchangeRate,
    decimal? SettledAmount,
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
        var paymentAmount = Math.Round(cmd.Amount, 2, MidpointRounding.AwayFromZero);
        var received = cmd.ReceivedUtc ?? DateTime.UtcNow;

        var snap = await _db.BookingFinancialSnapshots.FirstOrDefaultAsync(s => s.BookingId == cmd.BookingId, ct);
        if (snap is null)
        {
            var initialCurrency = string.IsNullOrWhiteSpace(cmd.Currency) ? "EUR" : cmd.Currency.ToUpperInvariant();
            snap = new BookingFinancialSnapshot
            {
                Id = Guid.NewGuid(),
                BookingId = cmd.BookingId,
                BranchId = Guid.Empty,
                CustomerId = Guid.Empty,
                TourId = Guid.Empty,
                TourTypeCode = "EXPEDITION",
                Gross = paymentAmount,
                Discount = 0m,
                Tax = 0m,
                Net = paymentAmount,
                SupplierCost = 0m,
                Margin = paymentAmount,
                Paid = 0m,
                Due = paymentAmount,
                Currency = initialCurrency,
                Status = "Confirmed",
                BookingDateUtc = received,
                UpdatedUtc = DateTime.UtcNow
            };
            _db.BookingFinancialSnapshots.Add(snap);
        }

        var paymentCurrency = string.IsNullOrWhiteSpace(cmd.Currency) ? snap.Currency : cmd.Currency.ToUpperInvariant();
        var exchangeRate = cmd.ExchangeRate ?? 1.0m;
        if (exchangeRate <= 0) exchangeRate = 1.0m;

        decimal settledAmount;
        if (cmd.SettledAmount.HasValue && cmd.SettledAmount.Value > 0)
        {
            settledAmount = Math.Round(cmd.SettledAmount.Value, 2, MidpointRounding.AwayFromZero);
        }
        else if (string.Equals(paymentCurrency, snap.Currency, StringComparison.OrdinalIgnoreCase))
        {
            settledAmount = paymentAmount;
        }
        else
        {
            settledAmount = Math.Round(paymentAmount / exchangeRate, 2, MidpointRounding.AwayFromZero);
        }

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            BookingId = snap.BookingId,
            BranchId = snap.BranchId,
            CustomerId = snap.CustomerId,
            Amount = paymentAmount,
            Currency = paymentCurrency,
            ExchangeRate = exchangeRate,
            SettledAmount = settledAmount,
            Method = string.IsNullOrWhiteSpace(cmd.Method) ? "Card" : cmd.Method,
            Reference = cmd.Reference,
            ReceivedUtc = received,
            CreatedBy = cmd.CreatedBy
        };
        _db.Payments.Add(payment);

        var fx = await FxResolver.ResolveRateAsync(_db, snap.Currency, received, ct);
        _db.JournalEntries.Add(LedgerPosting.PaymentReceipt(
            snap.BookingId, snap.BranchId, settledAmount, snap.Currency, fx, received, payment.Id.ToString()));

        var total = snap.Net + snap.Tax;
        snap.Paid += settledAmount;
        snap.Due = Math.Max(0m, total - snap.Paid);
        snap.UpdatedUtc = DateTime.UtcNow;

        await AddCollectedAsync(snap.BranchId, received.Date, snap.Currency, settledAmount, ct);

        _outbox.Enqueue(new PaymentRecorded
        {
            OccurredUtc = received,
            PaymentId = payment.Id,
            BookingId = snap.BookingId,
            BranchId = snap.BranchId,
            Amount = settledAmount,
            Currency = snap.Currency,
            CumulativePaid = snap.Paid,
            BookingTotal = total,
            Method = payment.Method,
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
