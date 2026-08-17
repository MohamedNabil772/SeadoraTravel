namespace Seadora.Content.Domain.Entities;

public class Nationality
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty; // e.g. "US", "DE"
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
