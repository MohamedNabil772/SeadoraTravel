namespace Seadora.Content.Domain.Entities;

public class Category
{
    public Guid Id { get; set; }
    
    // Localized Name (e.g., "en" -> "Sea & Diving")
    public Dictionary<string, string> Names { get; set; } = new();
    
    public string Icon { get; set; } = string.Empty; // Emoji or CSS class
    
    public ICollection<Tour> Tours { get; set; } = new List<Tour>();
}
