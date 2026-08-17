namespace Seadora.Content.Domain.Entities;

public class Category
{
    public Guid Id { get; set; }
    
    // Localized Name (e.g., "en" -> "Sea & Diving")
    public Dictionary<string, string> Names { get; set; } = new();
    public Dictionary<string, string> Descriptions { get; set; } = new();
    
    public string IconName { get; set; } = string.Empty; // Emoji or CSS class
    public int Order { get; set; }
    public string CoverImageUrl { get; set; } = string.Empty;
    
    public ICollection<Tour> Tours { get; set; } = new List<Tour>();
}
