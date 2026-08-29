using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Application.Common.Interfaces;

namespace Seadora.Content.Application.Tours.Queries.Admin;

public record TourFavoriteStatDto(
    Guid Id,
    string Title,
    string DestinationName,
    string CategoryName,
    decimal Price,
    string Currency,
    int FavoriteCount,
    decimal Rating,
    int ReviewCount,
    string ImageUrl
);

public record GetTourFavoritesLeaderboardQuery(int Limit = 20) : IRequest<List<TourFavoriteStatDto>>;

public class GetTourFavoritesLeaderboardQueryHandler : IRequestHandler<GetTourFavoritesLeaderboardQuery, List<TourFavoriteStatDto>>
{
    private readonly IContentDbContext _context;

    public GetTourFavoritesLeaderboardQueryHandler(IContentDbContext context)
    {
        _context = context;
    }

    public async Task<List<TourFavoriteStatDto>> Handle(GetTourFavoritesLeaderboardQuery request, CancellationToken cancellationToken)
    {
        var tours = await _context.Tours
            .Include(t => t.Destination)
            .Include(t => t.Category)
            .AsNoTracking()
            .OrderByDescending(t => t.FavoriteCount)
            .ThenByDescending(t => t.Rating)
            .Take(request.Limit)
            .ToListAsync(cancellationToken);

        return tours.Select(t => new TourFavoriteStatDto(
            t.Id,
            t.Names != null && t.Names.ContainsKey("en") ? t.Names["en"] : (t.Names?.Values.FirstOrDefault() ?? "Experience"),
            t.Destination?.Names != null && t.Destination.Names.ContainsKey("en") ? t.Destination.Names["en"] : (t.Destination?.Names?.Values.FirstOrDefault() ?? "Destination"),
            t.Category?.Names != null && t.Category.Names.ContainsKey("en") ? t.Category.Names["en"] : (t.Category?.Names?.Values.FirstOrDefault() ?? "Category"),
            t.Price,
            t.Currency ?? "EUR",
            t.FavoriteCount,
            t.Rating,
            t.ReviewCount,
            t.MediaUrls?.FirstOrDefault() ?? t.ImageUrl ?? string.Empty
        )).ToList();
    }
}
