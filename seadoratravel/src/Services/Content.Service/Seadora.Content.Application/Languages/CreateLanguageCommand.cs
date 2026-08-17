using MediatR;
using Seadora.Content.Application.Common.Interfaces;
using Seadora.Content.Domain.Entities;

namespace Seadora.Content.Application.Languages;

public record CreateLanguageCommand(string Code, string Name, string NativeName, bool IsActive) : IRequest<Guid>;

public class CreateLanguageCommandHandler : IRequestHandler<CreateLanguageCommand, Guid>
{
    private readonly IContentDbContext _context;

    public CreateLanguageCommandHandler(IContentDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateLanguageCommand request, CancellationToken cancellationToken)
    {
        var language = new Language
        {
            Id = Guid.NewGuid(),
            Code = request.Code,
            Name = request.Name,
            NativeName = request.NativeName,
            IsActive = request.IsActive
        };

        _context.Languages.Add(language);
        await _context.SaveChangesAsync(cancellationToken);

        return language.Id;
    }
}
