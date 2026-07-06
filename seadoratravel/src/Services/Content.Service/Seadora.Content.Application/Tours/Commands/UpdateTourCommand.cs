using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Application.Common.Interfaces;

namespace Seadora.Content.Application.Tours.Commands;

public record UpdateTourCommand(
    Guid Id,
    Dictionary<string, string> Names,
    Dictionary<string, string> Descriptions,
    decimal Price,
    string Duration,
    List<string> Includes,
    string ImageUrl,
    string Emoji,
    string BgGradient,
    string Badge,
    Guid DestinationId,
    Guid CategoryId
) : IRequest<Unit>;

public class UpdateTourCommandHandler : IRequestHandler<UpdateTourCommand, Unit>
{
    private readonly IContentDbContext _context;

    public UpdateTourCommandHandler(IContentDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateTourCommand request, CancellationToken cancellationToken)
    {
        var tour = await _context.Tours
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (tour == null)
        {
            throw new KeyNotFoundException("Tour not found.");
        }

        if (request.Names == null || request.Names.Count == 0)
        {
            throw new ArgumentException("Tour name is required.");
        }

        var destinationExists = await _context.Destinations.AnyAsync(d => d.Id == request.DestinationId, cancellationToken);
        if (!destinationExists)
        {
            throw new ArgumentException("Invalid DestinationId.");
        }

        var categoryExists = await _context.Categories.AnyAsync(c => c.Id == request.CategoryId, cancellationToken);
        if (!categoryExists)
        {
            throw new ArgumentException("Invalid CategoryId.");
        }

        tour.Names = request.Names;
        tour.Descriptions = request.Descriptions ?? new Dictionary<string, string>();
        tour.Price = request.Price;
        tour.Duration = request.Duration;
        tour.Includes = request.Includes ?? new List<string>();
        tour.ImageUrl = request.ImageUrl;
        tour.Emoji = request.Emoji;
        tour.BgGradient = request.BgGradient;
        tour.Badge = request.Badge;
        tour.DestinationId = request.DestinationId;
        tour.CategoryId = request.CategoryId;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
