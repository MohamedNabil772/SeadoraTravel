using MediatR;
using Seadora.Content.Application.Common.Interfaces;
using Seadora.Content.Domain.Entities;

namespace Seadora.Content.Application.Nationalities;

public record CreateNationalityCommand(string Code, string Name, bool IsActive) : IRequest<Guid>;

public class CreateNationalityCommandHandler : IRequestHandler<CreateNationalityCommand, Guid>
{
    private readonly IContentDbContext _context;

    public CreateNationalityCommandHandler(IContentDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateNationalityCommand request, CancellationToken cancellationToken)
    {
        var nationality = new Nationality
        {
            Id = Guid.NewGuid(),
            Code = request.Code,
            Name = request.Name,
            IsActive = request.IsActive
        };

        _context.Nationalities.Add(nationality);
        await _context.SaveChangesAsync(cancellationToken);

        return nationality.Id;
    }
}
