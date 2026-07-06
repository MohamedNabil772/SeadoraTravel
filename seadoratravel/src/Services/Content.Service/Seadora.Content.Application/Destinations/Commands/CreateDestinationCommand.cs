using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Seadora.Content.Application.Common.Interfaces;
using Seadora.Content.Domain.Entities;

namespace Seadora.Content.Application.Destinations.Commands;

public record CreateDestinationCommand(
    Dictionary<string, string> Names,
    Dictionary<string, string> Descriptions,
    string ImageUrl,
    string Flag
) : IRequest<Guid>;

public class CreateDestinationCommandHandler : IRequestHandler<CreateDestinationCommand, Guid>
{
    private readonly IContentDbContext _context;

    public CreateDestinationCommandHandler(IContentDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateDestinationCommand request, CancellationToken cancellationToken)
    {
        if (request.Names == null || request.Names.Count == 0)
        {
            throw new ArgumentException("Destination name is required.");
        }

        var destination = new Destination
        {
            Id = Guid.NewGuid(),
            Names = request.Names,
            Descriptions = request.Descriptions ?? new Dictionary<string, string>(),
            ImageUrl = request.ImageUrl,
            Flag = request.Flag
        };

        _context.Destinations.Add(destination);
        await _context.SaveChangesAsync(cancellationToken);

        return destination.Id;
    }
}
