using MediatR;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Application.Common.Interfaces;
using Seadora.Content.Domain.Entities;

namespace Seadora.Content.Application.Currencies;

public record SyncLiveExchangeRatesCommand() : IRequest<List<Currency>>;

public class SyncLiveExchangeRatesCommandHandler : IRequestHandler<SyncLiveExchangeRatesCommand, List<Currency>>
{
    private readonly IContentDbContext _context;
    private readonly IExchangeRateService _exchangeRateService;

    public SyncLiveExchangeRatesCommandHandler(IContentDbContext context, IExchangeRateService exchangeRateService)
    {
        _context = context;
        _exchangeRateService = exchangeRateService;
    }

    public async Task<List<Currency>> Handle(SyncLiveExchangeRatesCommand request, CancellationToken cancellationToken)
    {
        var currencies = await _context.Currencies.ToListAsync(cancellationToken);
        var baseCurrency = currencies.FirstOrDefault(c => c.IsBase)?.Code ?? "EUR";

        var liveRates = await _exchangeRateService.GetExchangeRatesAsync(baseCurrency, cancellationToken);

        if (liveRates != null && liveRates.Count > 0)
        {
            var now = DateTime.UtcNow;
            foreach (var currency in currencies)
            {
                if (currency.IsBase)
                {
                    currency.ExchangeRate = 1.0m;
                    currency.LiveExchangeRate = 1.0m;
                    currency.LastRateSyncAt = now;
                    continue;
                }

                if (liveRates.TryGetValue(currency.Code, out var liveRate))
                {
                    currency.LiveExchangeRate = liveRate;
                    currency.LastRateSyncAt = now;

                    // Automatically update exchange rate unless the user has manually overridden it
                    if (!currency.IsManualRate)
                    {
                        currency.ExchangeRate = liveRate;
                    }
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        return currencies.OrderByDescending(c => c.IsBase).ThenBy(c => c.Code).ToList();
    }
}
