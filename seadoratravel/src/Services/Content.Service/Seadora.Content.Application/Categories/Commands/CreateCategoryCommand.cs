using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Seadora.Content.Application.Common.Interfaces;
using Seadora.Content.Domain.Entities;

namespace Seadora.Content.Application.Categories.Commands;

public record CreateCategoryCommand(Dictionary<string, string> Names, string Icon) : IRequest<Guid>;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Guid>
{
    private readonly IContentDbContext _context;

    public CreateCategoryCommandHandler(IContentDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        if (request.Names == null || request.Names.Count == 0)
        {
            throw new ArgumentException("Category name is required.");
        }

        var category = new Category
        {
            Id = Guid.NewGuid(),
            Names = request.Names,
            Icon = request.Icon
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);

        return category.Id;
    }
}
