using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Application.Common.Interfaces;

namespace Seadora.Content.Application.Categories.Commands;

public record UpdateCategoryCommand(Guid Id, Dictionary<string, string> Names, string Icon) : IRequest<Unit>;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Unit>
{
    private readonly IContentDbContext _context;

    public UpdateCategoryCommandHandler(IContentDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (category == null)
        {
            throw new KeyNotFoundException("Category not found.");
        }

        if (request.Names == null || request.Names.Count == 0)
        {
            throw new ArgumentException("Category name is required.");
        }

        category.Names = request.Names;
        category.Icon = request.Icon;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
