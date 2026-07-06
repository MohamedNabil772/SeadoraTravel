using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Application.Common.Interfaces;

namespace Seadora.Content.Application.Tours.Commands;

public record DeleteTourCommand(Guid Id) : IRequest<Unit>;

public class DeleteTourCommandHandler : IRequestHandler<DeleteTourCommand, Unit>
{
    private readonly IContentDbContext _context;

    public DeleteTourCommandHandler(IContentDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteTourCommand request, CancellationToken cancellationToken)
    {
        var tour = await _context.Tours
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (tour == null)
        {
            throw new KeyNotFoundException("Tour not found.");
        }

        _context.Tours.Remove(tour);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
