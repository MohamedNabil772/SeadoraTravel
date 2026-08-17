using MediatR;
using Seadora.Content.Application.Common.Interfaces;

namespace Seadora.Content.Application.Categories.Commands;

public record DeleteCategoryCommand(Guid Id) : IRequest<Unit>;

public class DeleteCategoryCommandHandler(IContentDbContext context) : IRequestHandler<DeleteCategoryCommand, Unit>
{
    public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await context.Categories.FindAsync(new object[] { request.Id }, cancellationToken);
        if (category == null) throw new KeyNotFoundException("Category not found");
        
        context.Categories.Remove(category);
        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
