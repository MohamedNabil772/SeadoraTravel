using MediatR;
using Seadora.Content.Application.Common.Interfaces;
using Mapster;

namespace Seadora.Content.Application.Destinations.Commands;

public record UpdateDestinationCommand(
    Guid Id,
    Dictionary<string, string> Names,
    Dictionary<string, string> Descriptions,
    Dictionary<string, string> Highlights,
    string ImageUrl,
    string FlagEmoji) : IRequest<Unit>;

public class UpdateDestinationCommandHandler(IContentDbContext context) : IRequestHandler<UpdateDestinationCommand, Unit>
{
    public async Task<Unit> Handle(UpdateDestinationCommand request, CancellationToken cancellationToken)
    {
        var destination = await context.Destinations.FindAsync(new object[] { request.Id }, cancellationToken);
        if (destination == null) throw new KeyNotFoundException("Destination not found");
        
        destination.Names = request.Names ?? new();
        destination.Descriptions = request.Descriptions ?? new();
        destination.Highlights = request.Highlights ?? new();
        destination.ImageUrl = request.ImageUrl ?? "";
        destination.FlagEmoji = request.FlagEmoji ?? "";
        
        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
