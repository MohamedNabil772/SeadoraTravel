using MediatR;
using Seadora.Content.Application.Common.Interfaces;
using Seadora.Content.Application.DTOs;
using Mapster;

namespace Seadora.Content.Application.Categories.Queries;

public record GetCategoryByIdQuery(Guid Id) : IRequest<CategoryDto>;

public class GetCategoryByIdQueryHandler(IContentDbContext context) : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
{
    public async Task<CategoryDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await context.Categories.FindAsync(new object[] { request.Id }, cancellationToken);
        if (category == null) throw new KeyNotFoundException("Category not found");
        
        return category.Adapt<CategoryDto>();
    }
}
