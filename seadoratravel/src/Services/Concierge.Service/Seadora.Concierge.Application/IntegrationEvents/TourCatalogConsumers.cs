using MassTransit;
using Seadora.Contracts.Events;
using Seadora.Concierge.Application.Commands;
using Seadora.Concierge.Domain.Entities;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Seadora.Concierge.Application.IntegrationEvents;

public class TourCatalogConsumers : 
    IConsumer<TourPublished>,
    IConsumer<TourUpdated>,
    IConsumer<TourTypePolicyChanged>
{
    private readonly IConciergeDbContext _dbContext;

    public TourCatalogConsumers(IConciergeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<TourPublished> context)
    {
        var evt = context.Message;
        var existing = await _dbContext.TourCatalogIndices.FindAsync(evt.TourId);
        if (existing == null)
        {
            _dbContext.TourCatalogIndices.Add(new TourCatalogIndex
            {
                TourId = evt.TourId,
                BranchId = evt.BranchId,
                Slug = evt.TourTypeCode ?? string.Empty,
                Title = evt.TourTypeCode ?? string.Empty,
                PriceEur = evt.PriceFrom,
                IsActive = true,
                UpdatedUtc = System.DateTime.UtcNow,
                Names = "{}",
                Descriptions = "{}"
            });
        }
        await _dbContext.SaveChangesAsync(context.CancellationToken);
    }

    public async Task Consume(ConsumeContext<TourUpdated> context)
    {
        var evt = context.Message;
        var existing = await _dbContext.TourCatalogIndices.FindAsync(evt.TourId);
        if (existing != null)
        {
            existing.PriceEur = evt.PriceFrom;
            existing.UpdatedUtc = System.DateTime.UtcNow;
        }
        await _dbContext.SaveChangesAsync(context.CancellationToken);
    }

    public async Task Consume(ConsumeContext<TourTypePolicyChanged> context)
    {
        // Update any policies if needed, or invalidate cache
        await Task.CompletedTask;
    }
}
