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
        
        if (destination.Tours != null && destination.Tours.Any())
        {
            throw new InvalidOperationException("Cannot delete destination because it has tours attached.");
        }
        
        // Ensure we load tours if not already loaded depending on EF setup, 
        // better to query the db for tours:
        var hasTours = context.Tours.Any(t => t.DestinationId == request.Id);
        if (hasTours)
        {
            throw new InvalidOperationException("Cannot delete destination because it has tours attached.");
        }
        
        context.Destinations.Remove(destination);
        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
