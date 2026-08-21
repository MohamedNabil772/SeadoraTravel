namespace Seadora.Content.Domain.Entities;

public class Currency
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty; // e.g. "EUR", "USD", "EGP"
    public string Name { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public decimal ExchangeRate { get; set; } = 1.0m;
    public decimal? LiveExchangeRate { get; set; }
    public bool IsBase { get; set; }
    public bool IsManualRate { get; set; }
    public DateTime? LastRateSyncAt { get; set; }
    public bool IsActive { get; set; } = true;
}
