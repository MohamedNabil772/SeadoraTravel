using MediatR;
using Seadora.Content.Application.Common.Interfaces;
using Mapster;

namespace Seadora.Content.Application.Destinations.Commands;

public record UpdateDestinationCommand(
    Guid Id,
    Dictionary<string, string> Names,
    Dictionary<string, string> Descriptions,
    string ImageUrl,
    string Flag) : IRequest<Unit>;

public class UpdateDestinationCommandHandler(IContentDbContext context) : IRequestHandler<UpdateDestinationCommand, Unit>
{
    public async Task<Unit> Handle(UpdateDestinationCommand request, CancellationToken cancellationToken)
    {
        var destination = await context.Destinations.FindAsync(new object[] { request.Id }, cancellationToken);
        if (destination == null) throw new KeyNotFoundException("Destination not found");
        
        request.Adapt(destination);
        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
