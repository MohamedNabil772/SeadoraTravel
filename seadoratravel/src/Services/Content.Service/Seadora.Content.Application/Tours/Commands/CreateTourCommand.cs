using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Application.Common.Interfaces;
using Seadora.Content.Domain.Entities;

namespace Seadora.Content.Application.Tours.Commands;

public record CreateTourCommand(
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
) : IRequest<Guid>;

public class CreateTourCommandHandler : IRequestHandler<CreateTourCommand, Guid>
{
    private readonly IContentDbContext _context;

    public CreateTourCommandHandler(IContentDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateTourCommand request, CancellationToken cancellationToken)
    {
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

        var tour = new Tour
        {
            Id = Guid.NewGuid(),
            Names = request.Names,
            Descriptions = request.Descriptions ?? new Dictionary<string, string>(),
            Price = request.Price,
            Duration = request.Duration,
            Includes = request.Includes ?? new List<string>(),
            ImageUrl = request.ImageUrl,
            Emoji = request.Emoji,
            BgGradient = request.BgGradient,
            Badge = request.Badge,
            DestinationId = request.DestinationId,
            CategoryId = request.CategoryId
        };

        _context.Tours.Add(tour);
        await _context.SaveChangesAsync(cancellationToken);

        return tour.Id;
    }
}
