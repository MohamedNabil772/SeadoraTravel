using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Application.Common.Interfaces;

namespace Seadora.Content.Application.Languages;

public record ToggleLanguageStatusCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class ToggleLanguageStatusCommandHandler : IRequestHandler<ToggleLanguageStatusCommand, bool>
{
    private readonly IContentDbContext _context;

    public ToggleLanguageStatusCommandHandler(IContentDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(ToggleLanguageStatusCommand request, CancellationToken cancellationToken)
    {
        var language = await _context.Languages.FindAsync(new object[] { request.Id }, cancellationToken);

        if (language == null)
            return false;

        if (language.IsDefault && !request.IsActive)
            throw new InvalidOperationException("Cannot deactivate the default language.");

        language.IsActive = request.IsActive;
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
