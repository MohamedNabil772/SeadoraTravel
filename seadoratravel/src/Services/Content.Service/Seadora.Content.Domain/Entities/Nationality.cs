namespace Seadora.Content.Domain.Entities;

public class Nationality
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty; // e.g. "US", "EG", "DE"
    public string CountryName { get; set; } = string.Empty; // e.g. "United States"
    public string NationalityName { get; set; } = string.Empty; // e.g. "American"
    public string FlagEmoji { get; set; } = string.Empty; // e.g. "🇺🇸"
    public bool IsActive { get; set; } = true;
}
