using MediatR;
using Seadora.Content.Application.Common.Interfaces;
using Mapster;
using System.Collections.Generic;

namespace Seadora.Content.Application.Destinations.Commands;

public record UpdateDestinationCommand(
    Guid Id,
    Dictionary<string, string>? Names = null,
    Dictionary<string, string>? Descriptions = null,
    Dictionary<string, string>? Highlights = null,
    string? ImageUrl = null,
    string? FlagEmoji = null) : IRequest<Unit>;

public class UpdateDestinationCommandHandler(IContentDbContext context) : IRequestHandler<UpdateDestinationCommand, Unit>
{
    public async Task<Unit> Handle(UpdateDestinationCommand request, CancellationToken cancellationToken)
    {
        var destination = await context.Destinations.FindAsync(new object[] { request.Id }, cancellationToken);
        if (destination == null) throw new KeyNotFoundException("Destination not found");
        
        if (request.Names != null) destination.Names = request.Names;
        if (request.Descriptions != null) destination.Descriptions = request.Descriptions;
        if (request.Highlights != null) destination.Highlights = request.Highlights;
        if (request.ImageUrl != null) destination.ImageUrl = request.ImageUrl;
        if (request.FlagEmoji != null) destination.FlagEmoji = request.FlagEmoji;
        
        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
