using MediatR;
using Seadora.Content.Application.Common.Interfaces;

namespace Seadora.Content.Application.Destinations.Commands;

public record DeleteDestinationCommand(Guid Id) : IRequest<Unit>;

public class DeleteDestinationCommandHandler(IContentDbContext context) : IRequestHandler<DeleteDestinationCommand, Unit>
{
    public async Task<Unit> Handle(DeleteDestinationCommand request, CancellationToken cancellationToken)
    {
        var destination = await context.Destinations.FindAsync(new object[] { request.Id }, cancellationToken);
        if (destination == null) throw new KeyNotFoundException("Destination not found");
        
        context.Destinations.Remove(destination);
        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
