using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Application.Common.Interfaces;

namespace Seadora.Content.Application.Nationalities;

public record UpdateNationalityCommand(Guid Id, string Code, string Name, bool IsActive) : IRequest<bool>;

public class UpdateNationalityCommandHandler : IRequestHandler<UpdateNationalityCommand, bool>
{
    private readonly IContentDbContext _context;

    public UpdateNationalityCommandHandler(IContentDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateNationalityCommand request, CancellationToken cancellationToken)
    {
        var nationality = await _context.Nationalities.FindAsync(new object[] { request.Id }, cancellationToken);

        if (nationality == null)
            return false;

        nationality.Code = request.Code;
        nationality.Name = request.Name;
        nationality.IsActive = request.IsActive;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
