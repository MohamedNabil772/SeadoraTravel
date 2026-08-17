using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Application.Common.Interfaces;

namespace Seadora.Content.Application.Nationalities;

public record ToggleNationalityActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class ToggleNationalityActiveCommandHandler : IRequestHandler<ToggleNationalityActiveCommand, bool>
{
    private readonly IContentDbContext _context;

    public ToggleNationalityActiveCommandHandler(IContentDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(ToggleNationalityActiveCommand request, CancellationToken cancellationToken)
    {
        var nationality = await _context.Nationalities.FindAsync(new object[] { request.Id }, cancellationToken);

        if (nationality == null)
            return false;

        nationality.IsActive = request.IsActive;
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
