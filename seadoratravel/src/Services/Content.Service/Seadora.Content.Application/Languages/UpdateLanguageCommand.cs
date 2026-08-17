using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Application.Common.Interfaces;

namespace Seadora.Content.Application.Languages;

public record UpdateLanguageCommand(Guid Id, string Code, string Name, string NativeName, bool IsActive) : IRequest<bool>;

public class UpdateLanguageCommandHandler : IRequestHandler<UpdateLanguageCommand, bool>
{
    private readonly IContentDbContext _context;

    public UpdateLanguageCommandHandler(IContentDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateLanguageCommand request, CancellationToken cancellationToken)
    {
        var language = await _context.Languages.FindAsync(new object[] { request.Id }, cancellationToken);

        if (language == null)
            return false;

        language.Code = request.Code;
        language.Name = request.Name;
        language.NativeName = request.NativeName;
        language.IsActive = request.IsActive;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
