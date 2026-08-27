using Microsoft.EntityFrameworkCore;
using Seadora.Finance.Domain.Entities;

namespace Seadora.Finance.Application.Common.Interfaces;

public interface IFinanceDbContext
{
    DbSet<LedgerAccount> LedgerAccounts { get; }
    DbSet<JournalEntry> JournalEntries { get; }
    DbSet<JournalLine> JournalLines { get; }
    DbSet<Payment> Payments { get; }
    DbSet<SupplierSettlement> SupplierSettlements { get; }
    DbSet<CurrencyRate> CurrencyRates { get; }
    DbSet<BookingFinancialSnapshot> BookingFinancialSnapshots { get; }
    DbSet<RevenueDaily> RevenueDaily { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
