using MediatR;
using Seadora.Content.Application.Common.Interfaces;
using Seadora.Content.Domain.Entities;
using Mapster;

namespace Seadora.Content.Application.Destinations.Commands;

public record CreateDestinationCommand(
    Dictionary<string, string> Names,
    Dictionary<string, string> Descriptions,
    string ImageUrl,
    string Flag) : IRequest<Guid>;

public class CreateDestinationCommandHandler(IContentDbContext context) : IRequestHandler<CreateDestinationCommand, Guid>
{
    public async Task<Guid> Handle(CreateDestinationCommand request, CancellationToken cancellationToken)
    {
        var destination = request.Adapt<Destination>();
        destination.Id = Guid.NewGuid();
        context.Destinations.Add(destination);
        await context.SaveChangesAsync(cancellationToken);
        return destination.Id;
    }
}
