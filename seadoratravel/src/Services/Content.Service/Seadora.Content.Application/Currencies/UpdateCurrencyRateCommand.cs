using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Application.Common.Interfaces;

namespace Seadora.Content.Application.Currencies;

public record UpdateCurrencyRateCommand(Guid Id, decimal ExchangeRate) : IRequest<bool>;

public class UpdateCurrencyRateCommandHandler : IRequestHandler<UpdateCurrencyRateCommand, bool>
{
    private readonly IContentDbContext _context;

    public UpdateCurrencyRateCommandHandler(IContentDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateCurrencyRateCommand request, CancellationToken cancellationToken)
    {
        var currency = await _context.Currencies.FindAsync(new object[] { request.Id }, cancellationToken);

        if (currency == null)
            return false;

        currency.ExchangeRate = request.ExchangeRate;
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
