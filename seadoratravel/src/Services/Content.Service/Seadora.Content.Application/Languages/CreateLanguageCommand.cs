using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Application.Common.Interfaces;
using Seadora.Content.Domain.Entities;

namespace Seadora.Content.Application.Languages;

public record CreateLanguageCommand(string Code, string Name, string NativeName, string FlagEmoji, bool IsRtl, bool IsDefault, int Order, bool IsActive) : IRequest<Guid>;

public class CreateLanguageCommandHandler : IRequestHandler<CreateLanguageCommand, Guid>
{
    private readonly IContentDbContext _context;

    public CreateLanguageCommandHandler(IContentDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateLanguageCommand request, CancellationToken cancellationToken)
    {
        if (request.IsDefault)
        {
            var defaultLanguages = await _context.Languages.Where(l => l.IsDefault).ToListAsync(cancellationToken);
            foreach (var lang in defaultLanguages)
            {
                lang.IsDefault = false;
            }
        }

        var language = new Language
        {
            Id = Guid.NewGuid(),
            Code = request.Code,
            Name = request.Name,
            NativeName = request.NativeName,
            FlagEmoji = request.FlagEmoji,
            IsRtl = request.IsRtl,
            IsDefault = request.IsDefault,
            Order = request.Order,
            IsActive = request.IsActive
        };

        _context.Languages.Add(language);
        await _context.SaveChangesAsync(cancellationToken);

        return language.Id;
    }
}
