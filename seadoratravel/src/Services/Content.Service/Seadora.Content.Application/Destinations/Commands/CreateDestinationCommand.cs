using MediatR;
using Seadora.Content.Application.Common.Interfaces;
using Seadora.Content.Domain.Entities;
using Mapster;
using System.Collections.Generic;

namespace Seadora.Content.Application.Destinations.Commands;

public record CreateDestinationCommand(
    Dictionary<string, string> Names,
    Dictionary<string, string>? Descriptions = null,
    Dictionary<string, string>? Highlights = null,
    string? ImageUrl = null,
    string? FlagEmoji = null) : IRequest<Guid>;

public class CreateDestinationCommandHandler(IContentDbContext context) : IRequestHandler<CreateDestinationCommand, Guid>
{
    public async Task<Guid> Handle(CreateDestinationCommand request, CancellationToken cancellationToken)
    {
        var destination = new Destination
        {
            Id = Guid.NewGuid(),
            Names = request.Names ?? new(),
            Descriptions = request.Descriptions ?? new(),
            Highlights = request.Highlights ?? new(),
            ImageUrl = request.ImageUrl ?? "",
            FlagEmoji = request.FlagEmoji ?? ""
        };
        context.Destinations.Add(destination);
        await context.SaveChangesAsync(cancellationToken);
        return destination.Id;
    }
}
