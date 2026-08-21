using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Application.Common.Interfaces;

namespace Seadora.Content.Application.Nationalities;

public record DeleteNationalityCommand(Guid Id) : IRequest<bool>;

public class DeleteNationalityCommandHandler : IRequestHandler<DeleteNationalityCommand, bool>
{
    private readonly IContentDbContext _context;

    public DeleteNationalityCommandHandler(IContentDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteNationalityCommand request, CancellationToken cancellationToken)
    {
        var nationality = await _context.Nationalities.FindAsync(new object[] { request.Id }, cancellationToken);

        if (nationality == null)
            return false;

        _context.Nationalities.Remove(nationality);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
