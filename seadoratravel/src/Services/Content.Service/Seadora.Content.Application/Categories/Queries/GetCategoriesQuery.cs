using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Application.Common.Interfaces;
using Seadora.Content.Application.DTOs;
using Mapster;

namespace Seadora.Content.Application.Categories.Queries;

public record GetCategoriesQuery() : IRequest<List<CategoryDto>>;

public class GetCategoriesQueryHandler(IContentDbContext context) : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
{
    public async Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await context.Categories
            .AsNoTracking()
            .OrderBy(c => c.Order)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Names = c.Names,
                Descriptions = c.Descriptions,
                IconName = c.IconName,
                CustomIconUrl = c.CustomIconUrl,
                Order = c.Order,
                CoverImageUrl = c.CoverImageUrl,
                TourCount = context.Tours.Count(t => t.CategoryId == c.Id)
            })
            .ToListAsync(cancellationToken);

        return categories;
    }
}
