using Microsoft.EntityFrameworkCore;
using Seadora.Common.Messaging.Idempotency;
using Seadora.Finance.Domain;
using Seadora.Finance.Domain.Entities;

namespace Seadora.Finance.Infrastructure.Persistence;

public class FinanceDbContext : DbContext, IProcessedMessageDbContext
{
    private static readonly DateTime SeedAsOfUtc = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private static readonly Guid IdentityRateId = Guid.Parse("c0000000-0000-0000-0000-000000000001");

    public FinanceDbContext(DbContextOptions<FinanceDbContext> options) : base(options) { }

    public DbSet<LedgerAccount> LedgerAccounts => Set<LedgerAccount>();
    public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();
    public DbSet<JournalLine> JournalLines => Set<JournalLine>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<SupplierSettlement> SupplierSettlements => Set<SupplierSettlement>();
    public DbSet<CurrencyRate> CurrencyRates => Set<CurrencyRate>();
    public DbSet<BookingFinancialSnapshot> BookingFinancialSnapshots => Set<BookingFinancialSnapshot>();
    public DbSet<RevenueDaily> RevenueDaily => Set<RevenueDaily>();
    public DbSet<ProcessedMessage> ProcessedMessages => Set<ProcessedMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FinanceDbContext).Assembly);

        modelBuilder.Entity<LedgerAccount>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Code).IsRequired().HasMaxLength(10);
            entity.Property(a => a.Name).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Type).HasConversion<string>().HasMaxLength(20);
            entity.Property(a => a.NormalSide).HasConversion<string>().HasMaxLength(10);
            entity.HasIndex(a => a.Code).IsUnique();
            entity.HasData(ChartOfAccounts.All.Select(a => new LedgerAccount
            {
                Id = a.Id,
                Code = a.Code,
                Name = a.Name,
                Type = a.Type,
                NormalSide = a.NormalSide
            }));
        });

        modelBuilder.Entity<JournalEntry>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
            entity.Property(e => e.SourceEventId).HasMaxLength(200);
            entity.HasIndex(e => e.BranchId);
            entity.HasIndex(e => e.BookingId);
            entity.HasMany(e => e.Lines)
                  .WithOne()
                  .HasForeignKey(l => l.JournalEntryId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.Metadata.FindNavigation(nameof(JournalEntry.Lines))?
                  .SetPropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<JournalLine>(entity =>
        {
            entity.HasKey(l => l.Id);
            entity.Property(l => l.Currency).IsRequired().HasMaxLength(3);
            entity.Property(l => l.Debit).HasPrecision(18, 2);
            entity.Property(l => l.Credit).HasPrecision(18, 2);
            entity.Property(l => l.ReportingDebit).HasPrecision(18, 2);
            entity.Property(l => l.ReportingCredit).HasPrecision(18, 2);
            entity.Property(l => l.FxRate).HasPrecision(18, 6);
            entity.HasIndex(l => l.AccountId);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Amount).HasPrecision(18, 2);
            entity.Property(p => p.Currency).IsRequired().HasMaxLength(3);
            entity.Property(p => p.Method).HasConversion<string>().HasMaxLength(20);
            entity.Property(p => p.Reference).HasMaxLength(200);
            entity.Property(p => p.CreatedBy).HasMaxLength(200);
            entity.HasIndex(p => p.BookingId);
        });

        modelBuilder.Entity<SupplierSettlement>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.AccruedAmount).HasPrecision(18, 2);
            entity.Property(s => s.PaidAmount).HasPrecision(18, 2);
            entity.Property(s => s.Currency).IsRequired().HasMaxLength(3);
            entity.Property(s => s.Status).HasConversion<string>().HasMaxLength(20);
            entity.HasIndex(s => new { s.SupplierId, s.PeriodStart, s.PeriodEnd });
        });

        modelBuilder.Entity<CurrencyRate>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.FromCurrency).IsRequired().HasMaxLength(3);
            entity.Property(r => r.ToCurrency).IsRequired().HasMaxLength(3);
            entity.Property(r => r.Rate).HasPrecision(18, 6);
            entity.HasIndex(r => new { r.FromCurrency, r.ToCurrency, r.AsOfUtc }).IsUnique();
            entity.HasData(new CurrencyRate
            {
                Id = IdentityRateId,
                FromCurrency = FinanceConstants.ReportingCurrency,
                ToCurrency = FinanceConstants.ReportingCurrency,
                Rate = 1.0m,
                AsOfUtc = SeedAsOfUtc
            });
        });

        // ponytail: pre-aggregated projections only. AR-aging buckets and dashboard KPIs are
        // derived at query time from these two tables - no extra tables to keep in sync.
        modelBuilder.Entity<BookingFinancialSnapshot>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.TourTypeCode).HasMaxLength(50);
            entity.Property(s => s.Currency).IsRequired().HasMaxLength(3);
            entity.Property(s => s.Status).IsRequired().HasMaxLength(50);
            entity.Property(s => s.Gross).HasPrecision(18, 2);
            entity.Property(s => s.Discount).HasPrecision(18, 2);
            entity.Property(s => s.Tax).HasPrecision(18, 2);
            entity.Property(s => s.Net).HasPrecision(18, 2);
            entity.Property(s => s.SupplierCost).HasPrecision(18, 2);
            entity.Property(s => s.Margin).HasPrecision(18, 2);
            entity.Property(s => s.Paid).HasPrecision(18, 2);
            entity.Property(s => s.Due).HasPrecision(18, 2);
            entity.HasIndex(s => s.BookingId).IsUnique();
            entity.HasIndex(s => s.BranchId);
        });

        modelBuilder.Entity<RevenueDaily>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Day).HasColumnType("date");
            entity.Property(r => r.Currency).IsRequired().HasMaxLength(3);
            entity.Property(r => r.Recognized).HasPrecision(18, 2);
            entity.Property(r => r.Collected).HasPrecision(18, 2);
            entity.Property(r => r.Refunds).HasPrecision(18, 2);
            entity.Property(r => r.SupplierCost).HasPrecision(18, 2);
            entity.Property(r => r.Margin).HasPrecision(18, 2);
            entity.HasIndex(r => new { r.BranchId, r.Day, r.Currency }).IsUnique();
        });

        modelBuilder.Entity<ProcessedMessage>().HasKey(p => new { p.MessageId, p.ConsumerName });
    }
}
