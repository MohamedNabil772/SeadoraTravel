using MediatR;
using Seadora.Content.Application.Common.Interfaces;
using Seadora.Content.Domain.Entities;
using Mapster;
using System.Collections.Generic;

namespace Seadora.Content.Application.Categories.Commands;

public record CreateCategoryCommand(
    Dictionary<string, string> Names,
    Dictionary<string, string>? Descriptions = null,
    string? IconName = null,
    string? CustomIconUrl = null,
    int Order = 0,
    string? CoverImageUrl = null) : IRequest<Guid>;

public class CreateCategoryCommandHandler(IContentDbContext context) : IRequestHandler<CreateCategoryCommand, Guid>
{
    public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = request.Adapt<Category>();
        category.Id = Guid.NewGuid();
        if (category.Descriptions == null) category.Descriptions = new();
        if (category.CoverImageUrl == null) category.CoverImageUrl = string.Empty;
        context.Categories.Add(category);
        await context.SaveChangesAsync(cancellationToken);
        return category.Id;
    }
}
