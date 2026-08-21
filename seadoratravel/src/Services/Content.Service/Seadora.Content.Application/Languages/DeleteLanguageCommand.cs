using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Application.Common.Interfaces;

namespace Seadora.Content.Application.Languages;

public record DeleteLanguageCommand(Guid Id) : IRequest<bool>;

public class DeleteLanguageCommandHandler : IRequestHandler<DeleteLanguageCommand, bool>
{
    private readonly IContentDbContext _context;

    public DeleteLanguageCommandHandler(IContentDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteLanguageCommand request, CancellationToken cancellationToken)
    {
        var language = await _context.Languages.FindAsync(new object[] { request.Id }, cancellationToken);

        if (language == null)
            return false;

        if (language.IsDefault)
            throw new InvalidOperationException("Cannot delete the default language.");

        _context.Languages.Remove(language);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
