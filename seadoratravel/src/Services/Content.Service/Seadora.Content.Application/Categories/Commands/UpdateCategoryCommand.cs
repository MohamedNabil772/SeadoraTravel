using MediatR;
using Seadora.Content.Application.Common.Interfaces;
using Seadora.Content.Domain.Entities;
using Mapster;
using System.Collections.Generic;

namespace Seadora.Content.Application.Categories.Commands;

public record UpdateCategoryCommand(
    Guid Id,
    Dictionary<string, string>? Names = null,
    Dictionary<string, string>? Descriptions = null,
    string? IconName = null,
    string? CustomIconUrl = null,
    int Order = 0,
    string? CoverImageUrl = null) : IRequest<Unit>;

public class UpdateCategoryCommandHandler(IContentDbContext context) : IRequestHandler<UpdateCategoryCommand, Unit>
{
    public async Task<Unit> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await context.Categories.FindAsync(new object[] { request.Id }, cancellationToken);
        if (category == null) throw new KeyNotFoundException("Category not found");
        
        if (request.Names != null) category.Names = request.Names;
        if (request.Descriptions != null) category.Descriptions = request.Descriptions;
        if (request.IconName != null) category.IconName = request.IconName;
        if (request.CustomIconUrl != null) category.CustomIconUrl = request.CustomIconUrl;
        if (request.Order != 0) category.Order = request.Order;
        if (request.CoverImageUrl != null) category.CoverImageUrl = request.CoverImageUrl;

        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
