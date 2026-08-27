namespace Seadora.Finance.Domain.Entities;

/// <summary>A requested ledger line, before the entry validates and converts it.</summary>
public record JournalLineDraft(Guid AccountId, decimal Debit, decimal Credit, string Currency, decimal FxRate);
