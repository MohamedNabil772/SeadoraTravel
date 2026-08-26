using MassTransit;
using Microsoft.EntityFrameworkCore;
using Seadora.Booking.Application.Common.Interfaces;
using Seadora.Booking.Domain.Entities;
using Seadora.Common.Messaging.Idempotency;
using Seadora.Contracts.Events;

namespace Seadora.Booking.Application.Integration;

// ponytail: consumer depends on IBookingDbContext, not BookingDbContext — Infrastructure already
// references Application, so the concrete type would be a circular project reference.
public sealed class TourProjectionConsumers
    : IConsumer<TourPublished>, IConsumer<TourUpdated>, IConsumer<TourTypePolicyChanged>
{
    public const string PublishedConsumer = nameof(TourProjectionConsumers) + ":" + nameof(TourPublished);
    public const string UpdatedConsumer = nameof(TourProjectionConsumers) + ":" + nameof(TourUpdated);
    public const string PolicyConsumer = nameof(TourProjectionConsumers) + ":" + nameof(TourTypePolicyChanged);

    private readonly IBookingDbContext _db;
    private readonly IIdempotentConsumer _idem;

    public TourProjectionConsumers(IBookingDbContext db, IIdempotentConsumer idem)
    {
        _db = db;
        _idem = idem;
    }

    public Task Consume(ConsumeContext<TourPublished> ctx) =>
        HandleTourUpsertAsync(ctx.Message, PublishedConsumer, ctx.CancellationToken);

    public Task Consume(ConsumeContext<TourUpdated> ctx) =>
        HandleTourUpsertAsync(ToSnapshot(ctx.Message), UpdatedConsumer, ctx.CancellationToken);

    public Task Consume(ConsumeContext<TourTypePolicyChanged> ctx) =>
        HandlePolicyChangeAsync(ctx.Message, ctx.CancellationToken);

    public async Task HandleTourUpsertAsync(TourPublished evt, string consumerName = PublishedConsumer, CancellationToken ct = default)
    {
        if (await _idem.AlreadyProcessed(evt.Id, consumerName, ct)) return;

        var row = await _db.TourProjections.FirstOrDefaultAsync(p => p.TourId == evt.TourId, ct);
        if (row == null)
        {
            row = new TourProjection { TourId = evt.TourId };
            _db.TourProjections.Add(row);
        }

        row.BranchId = evt.BranchId;
        row.TourTypeCode = evt.TourTypeCode;
        row.AllocationModel = evt.AllocationModel;
        row.MinCapacity = evt.MinCapacity;
        row.MaxCapacity = evt.MaxCapacity;
        row.RequiresGuestDetails = evt.RequiresGuestDetails;
        row.RequiresPassport = evt.RequiresPassport;
        row.PayLaterAllowed = evt.PayLaterAllowed;
        row.PriceFrom = evt.PriceFrom;
        row.Currency = evt.Currency;
        row.UpdatedUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        await _idem.MarkProcessed(evt.Id, consumerName, ct);
    }

    public Task HandleTourUpsertAsync(TourUpdated evt, CancellationToken ct = default) =>
        HandleTourUpsertAsync(ToSnapshot(evt), UpdatedConsumer, ct);

    public async Task HandlePolicyChangeAsync(TourTypePolicyChanged evt, CancellationToken ct = default)
    {
        if (await _idem.AlreadyProcessed(evt.Id, PolicyConsumer, ct)) return;

        // capacity/price stay owned by the tour events
        var rows = await _db.TourProjections.Where(p => p.TourTypeCode == evt.Code).ToListAsync(ct);
        foreach (var p in rows)
        {
            p.AllocationModel = evt.AllocationModel;
            p.RequiresGuestDetails = evt.RequiresGuestDetails;
            p.RequiresPassport = evt.RequiresPassport;
            p.PayLaterAllowed = evt.PayLaterAllowed;
            p.UpdatedUtc = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync(ct);
        await _idem.MarkProcessed(evt.Id, PolicyConsumer, ct);
    }

    private static TourPublished ToSnapshot(TourUpdated e) => new()
    {
        Id = e.Id,
        OccurredUtc = e.OccurredUtc,
        TourId = e.TourId,
        BranchId = e.BranchId,
        TourTypeCode = e.TourTypeCode,
        AllocationModel = e.AllocationModel,
        MinCapacity = e.MinCapacity,
        MaxCapacity = e.MaxCapacity,
        RequiresGuestDetails = e.RequiresGuestDetails,
        RequiresPassport = e.RequiresPassport,
        PayLaterAllowed = e.PayLaterAllowed,
        PriceFrom = e.PriceFrom,
        Currency = e.Currency
    };
}
