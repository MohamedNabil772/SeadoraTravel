using System.Text.Json;
using Microsoft.Extensions.Logging;
using Seadora.Content.Application.Common.Interfaces;

namespace Seadora.Content.Infrastructure.Services;

public class ExchangeRateService : IExchangeRateService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ExchangeRateService> _logger;

    public ExchangeRateService(HttpClient httpClient, ILogger<ExchangeRateService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _httpClient.Timeout = TimeSpan.FromSeconds(10);
    }

    public async Task<Dictionary<string, decimal>?> GetExchangeRatesAsync(string baseCurrency = "EUR", CancellationToken cancellationToken = default)
    {
        try
        {
            // Primary free API: open.er-api.com
            var response = await _httpClient.GetAsync($"https://open.er-api.com/v6/latest/{baseCurrency}", cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                using var doc = JsonDocument.Parse(content);
                var root = doc.RootElement;
                if (root.TryGetProperty("result", out var resultProp) && resultProp.GetString() == "success" &&
                    root.TryGetProperty("rates", out var ratesProp))
                {
                    var rates = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
                    foreach (var prop in ratesProp.EnumerateObject())
                    {
                        if (prop.Value.TryGetDecimal(out var rateVal))
                        {
                            rates[prop.Name] = rateVal;
                        }
                    }
                    return rates;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Primary exchange rate API failed. Attempting fallback...");
        }

        try
        {
            // Fallback free API: frankfurter.app
            var response = await _httpClient.GetAsync($"https://api.frankfurter.app/latest?from={baseCurrency}", cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                using var doc = JsonDocument.Parse(content);
                var root = doc.RootElement;
                if (root.TryGetProperty("rates", out var ratesProp))
                {
                    var rates = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase)
                    {
                        [baseCurrency] = 1.0m
                    };
                    foreach (var prop in ratesProp.EnumerateObject())
                    {
                        if (prop.Value.TryGetDecimal(out var rateVal))
                        {
                            rates[prop.Name] = rateVal;
                        }
                    }
                    return rates;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch live exchange rates from all sources.");
        }

        return null;
    }
}
