using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Application.Common.Interfaces;

namespace Seadora.Content.Application.Currencies;

public record ResetCurrencyToLiveRateCommand(Guid Id) : IRequest<bool>;

public class ResetCurrencyToLiveRateCommandHandler : IRequestHandler<ResetCurrencyToLiveRateCommand, bool>
{
    private readonly IContentDbContext _context;

    public ResetCurrencyToLiveRateCommandHandler(IContentDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(ResetCurrencyToLiveRateCommand request, CancellationToken cancellationToken)
    {
        var currency = await _context.Currencies.FindAsync(new object[] { request.Id }, cancellationToken);

        if (currency == null)
            return false;

        if (currency.LiveExchangeRate.HasValue)
        {
            currency.ExchangeRate = currency.LiveExchangeRate.Value;
            currency.IsManualRate = false; // Remove manual override
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        return false;
    }
}
