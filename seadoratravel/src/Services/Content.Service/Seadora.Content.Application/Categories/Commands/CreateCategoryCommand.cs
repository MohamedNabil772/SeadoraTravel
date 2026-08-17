using MediatR;
using Seadora.Content.Application.Common.Interfaces;
using Seadora.Content.Domain.Entities;
using Mapster;

namespace Seadora.Content.Application.Categories.Commands;

public record CreateCategoryCommand(
    Dictionary<string, string> Names,
    Dictionary<string, string> Descriptions,
    string IconName,
    int Order,
    string CoverImageUrl) : IRequest<Guid>;

public class CreateCategoryCommandHandler(IContentDbContext context) : IRequestHandler<CreateCategoryCommand, Guid>
{
    public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = request.Adapt<Category>();
        category.Id = Guid.NewGuid();
        context.Categories.Add(category);
        await context.SaveChangesAsync(cancellationToken);
        return category.Id;
    }
}
