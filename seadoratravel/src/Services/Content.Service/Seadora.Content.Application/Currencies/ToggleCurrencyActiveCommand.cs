using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Application.Common.Interfaces;

namespace Seadora.Content.Application.Currencies;

public record ToggleCurrencyActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class ToggleCurrencyActiveCommandHandler : IRequestHandler<ToggleCurrencyActiveCommand, bool>
{
    private readonly IContentDbContext _context;

    public ToggleCurrencyActiveCommandHandler(IContentDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(ToggleCurrencyActiveCommand request, CancellationToken cancellationToken)
    {
        var currency = await _context.Currencies.FindAsync(new object[] { request.Id }, cancellationToken);

        if (currency == null)
            return false;

        currency.IsActive = request.IsActive;
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
