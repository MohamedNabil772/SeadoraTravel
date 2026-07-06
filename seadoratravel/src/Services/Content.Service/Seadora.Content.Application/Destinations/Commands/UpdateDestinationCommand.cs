using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Application.Common.Interfaces;

namespace Seadora.Content.Application.Destinations.Commands;

public record UpdateDestinationCommand(
    Guid Id,
    Dictionary<string, string> Names,
    Dictionary<string, string> Descriptions,
    string ImageUrl,
    string Flag
) : IRequest<Unit>;

public class UpdateDestinationCommandHandler : IRequestHandler<UpdateDestinationCommand, Unit>
{
    private readonly IContentDbContext _context;

    public UpdateDestinationCommandHandler(IContentDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateDestinationCommand request, CancellationToken cancellationToken)
    {
        var destination = await _context.Destinations
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

        if (destination == null)
        {
            throw new KeyNotFoundException("Destination not found.");
        }

        if (request.Names == null || request.Names.Count == 0)
        {
            throw new ArgumentException("Destination name is required.");
        }

        destination.Names = request.Names;
        destination.Descriptions = request.Descriptions ?? new Dictionary<string, string>();
        destination.ImageUrl = request.ImageUrl;
        destination.Flag = request.Flag;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
