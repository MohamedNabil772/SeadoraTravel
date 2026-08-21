using MediatR;
using Seadora.Content.Application.Common.Interfaces;
using Seadora.Content.Domain.Entities;
using Mapster;

namespace Seadora.Content.Application.Categories.Commands;

public record UpdateCategoryCommand(
    Guid Id,
    Dictionary<string, string> Names,
    Dictionary<string, string> Descriptions,
    string? IconName,
    string? CustomIconUrl,
    int Order,
    string CoverImageUrl) : IRequest<Unit>;

public class UpdateCategoryCommandHandler(IContentDbContext context) : IRequestHandler<UpdateCategoryCommand, Unit>
{
    public async Task<Unit> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await context.Categories.FindAsync(new object[] { request.Id }, cancellationToken);
        if (category == null) throw new KeyNotFoundException("Category not found");
        
        request.Adapt(category);
        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
