namespace Seadora.Content.Application.Common.Interfaces;

public interface IExchangeRateService
{
    Task<Dictionary<string, decimal>?> GetExchangeRatesAsync(string baseCurrency = "EUR", CancellationToken cancellationToken = default);
}
