using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Application.Common.Interfaces;

namespace Seadora.Content.Application.Categories.Commands;

public record CategoryOrderDto(Guid Id, int Order);

public record ReorderCategoriesCommand(List<CategoryOrderDto> Categories) : IRequest<Unit>;

public class ReorderCategoriesCommandHandler(IContentDbContext context) : IRequestHandler<ReorderCategoriesCommand, Unit>
{
    public async Task<Unit> Handle(ReorderCategoriesCommand request, CancellationToken cancellationToken)
    {
        var ids = request.Categories.Select(c => c.Id).ToList();
        var categories = await context.Categories.Where(c => ids.Contains(c.Id)).ToListAsync(cancellationToken);
        
        foreach (var category in categories)
        {
            var orderDto = request.Categories.First(c => c.Id == category.Id);
            category.Order = orderDto.Order;
        }
        
        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
