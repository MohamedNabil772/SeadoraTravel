using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Application.Common.Interfaces;

namespace Seadora.Content.Application.Tours.Commands;

public record ToggleTourFavoriteCommand(Guid Id, bool IsFavorite) : IRequest<int>;

public class ToggleTourFavoriteCommandHandler : IRequestHandler<ToggleTourFavoriteCommand, int>
{
    private readonly IContentDbContext _context;

    public ToggleTourFavoriteCommandHandler(IContentDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(ToggleTourFavoriteCommand request, CancellationToken cancellationToken)
    {
        var tour = await _context.Tours.FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);
        if (tour == null) return 0;

        if (request.IsFavorite)
        {
            tour.FavoriteCount++;
        }
        else
        {
            tour.FavoriteCount = Math.Max(0, tour.FavoriteCount - 1);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return tour.FavoriteCount;
    }
}
