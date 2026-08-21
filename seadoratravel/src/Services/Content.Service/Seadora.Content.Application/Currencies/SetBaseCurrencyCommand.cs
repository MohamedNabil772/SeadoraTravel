using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Application.Common.Interfaces;

namespace Seadora.Content.Application.Currencies;

public record SetBaseCurrencyCommand(Guid Id) : IRequest<bool>;

public class SetBaseCurrencyCommandHandler : IRequestHandler<SetBaseCurrencyCommand, bool>
{
    private readonly IContentDbContext _context;

    public SetBaseCurrencyCommandHandler(IContentDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(SetBaseCurrencyCommand request, CancellationToken cancellationToken)
    {
        var currencies = await _context.Currencies.ToListAsync(cancellationToken);
        var target = currencies.FirstOrDefault(c => c.Id == request.Id);

        if (target == null)
            return false;

        foreach (var c in currencies)
        {
            if (c.Id == target.Id)
            {
                c.IsBase = true;
                c.ExchangeRate = 1.0m;
                c.IsManualRate = false;
                c.IsActive = true;
            }
            else
            {
                c.IsBase = false;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
