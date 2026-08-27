namespace Seadora.Finance.Domain.Entities;

public class CurrencyRate
{
    public Guid Id { get; set; }
    public string FromCurrency { get; set; } = string.Empty;
    public string ToCurrency { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public DateTime AsOfUtc { get; set; }
}
