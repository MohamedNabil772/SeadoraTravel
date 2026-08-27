using Microsoft.EntityFrameworkCore;
using Seadora.Finance.Application.Common.Interfaces;
using Seadora.Finance.Domain;

namespace Seadora.Finance.Application.Common;

public static class FxResolver
{
    /// <summary>
    /// Rate from <paramref name="fromCurrency"/> to the reporting currency at <paramref name="asOf"/>.
    /// Reporting currency maps to 1. Missing rate falls back to 1.
    /// </summary>
    public static async Task<decimal> ResolveRateAsync(IFinanceDbContext db, string fromCurrency,
        DateTime asOf, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(fromCurrency) ||
            string.Equals(fromCurrency, FinanceConstants.ReportingCurrency, StringComparison.OrdinalIgnoreCase))
            return 1m;

        var rate = await db.CurrencyRates
            .Where(r => r.FromCurrency == fromCurrency
                        && r.ToCurrency == FinanceConstants.ReportingCurrency
                        && r.AsOfUtc <= asOf)
            .OrderByDescending(r => r.AsOfUtc)
            .Select(r => (decimal?)r.Rate)
            .FirstOrDefaultAsync(ct);

        // ponytail: missing-rate fallback keeps the ledger balanced in nominal terms until rates are seeded.
        return rate ?? 1m;
    }
}
