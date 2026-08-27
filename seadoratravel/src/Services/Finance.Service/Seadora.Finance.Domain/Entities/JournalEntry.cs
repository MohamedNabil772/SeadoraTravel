namespace Seadora.Finance.Domain.Entities;

/// <summary>
/// Append-only double-entry journal entry. Balanced at construction; there is no way to
/// add, remove or edit lines afterwards.
/// </summary>
// ponytail: corrections are a new reversing entry, never a mutation - a Reverse() helper is a
// few lines on top of Create() when the first correction use case shows up.
public class JournalEntry
{
    private readonly List<JournalLine> _lines = new();

    private JournalEntry() { }

    public Guid Id { get; private set; }
    public DateTime OccurredUtc { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public Guid BranchId { get; private set; }
    public Guid? BookingId { get; private set; }
    public string? SourceEventId { get; private set; }
    public DateTime CreatedUtc { get; private set; }

    public IReadOnlyList<JournalLine> Lines => _lines;

    public static JournalEntry Create(
        DateTime occurredUtc,
        string description,
        Guid branchId,
        Guid? bookingId,
        string? sourceEventId,
        IEnumerable<JournalLineDraft> lines)
    {
        var drafts = lines?.ToList() ?? throw new ArgumentNullException(nameof(lines));
        if (drafts.Count < 2)
            throw new ArgumentException("a journal entry needs at least 2 lines", nameof(lines));

        var entry = new JournalEntry
        {
            Id = Guid.NewGuid(),
            OccurredUtc = occurredUtc,
            Description = description,
            BranchId = branchId,
            BookingId = bookingId,
            SourceEventId = sourceEventId,
            CreatedUtc = DateTime.UtcNow
        };

        foreach (var draft in drafts)
        {
            if (draft.Debit < 0 || draft.Credit < 0)
                throw new ArgumentException("journal line amounts cannot be negative", nameof(lines));
            if ((draft.Debit > 0) == (draft.Credit > 0))
                throw new ArgumentException("journal line must have exactly one of debit or credit positive", nameof(lines));
            if (draft.FxRate <= 0)
                throw new ArgumentException("journal line fx rate must be positive", nameof(lines));
            if (string.IsNullOrWhiteSpace(draft.Currency))
                throw new ArgumentException("journal line currency is required", nameof(lines));

            entry._lines.Add(new JournalLine
            {
                Id = Guid.NewGuid(),
                JournalEntryId = entry.Id,
                AccountId = draft.AccountId,
                Debit = draft.Debit,
                Credit = draft.Credit,
                Currency = draft.Currency,
                FxRate = draft.FxRate,
                ReportingDebit = Math.Round(draft.Debit * draft.FxRate, 2, MidpointRounding.AwayFromZero),
                ReportingCredit = Math.Round(draft.Credit * draft.FxRate, 2, MidpointRounding.AwayFromZero)
            });
        }

        var debits = entry._lines.Sum(l => l.ReportingDebit);
        var credits = entry._lines.Sum(l => l.ReportingCredit);
        // ponytail: 0.01 tolerance absorbs per-line FX rounding; anything bigger is a real bug.
        if (Math.Abs(debits - credits) > 0.01m)
            throw new InvalidOperationException($"journal entry is not balanced: debits {debits} != credits {credits}");

        return entry;
    }
}
