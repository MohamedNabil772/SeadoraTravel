namespace Seadora.Content.Domain.Entities;

public class Destination
{
    public Guid Id { get; set; }
    
    // Localized Fields
    public Dictionary<string, string> Names { get; set; } = new();
    public Dictionary<string, string> Descriptions { get; set; } = new();
    public Dictionary<string, string> Highlights { get; set; } = new();
    
    public string ImageUrl { get; set; } = string.Empty;
    public string FlagEmoji { get; set; } = string.Empty;
    public ICollection<Tour> Tours { get; set; } = new List<Tour>();
}
