using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Application.Common.Interfaces;

namespace Seadora.Content.Application.Destinations.Commands;

public record DeleteDestinationCommand(Guid Id) : IRequest<Unit>;

public class DeleteDestinationCommandHandler : IRequestHandler<DeleteDestinationCommand, Unit>
{
    private readonly IContentDbContext _context;

    public DeleteDestinationCommandHandler(IContentDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteDestinationCommand request, CancellationToken cancellationToken)
    {
        var destination = await _context.Destinations
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

        if (destination == null)
        {
            throw new KeyNotFoundException("Destination not found.");
        }

        var hasTours = await _context.Tours.AnyAsync(t => t.DestinationId == request.Id, cancellationToken);
        if (hasTours)
        {
            throw new InvalidOperationException("Cannot delete destination with associated tours.");
        }

        _context.Destinations.Remove(destination);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
