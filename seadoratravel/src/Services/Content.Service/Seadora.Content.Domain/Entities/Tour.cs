namespace Seadora.Content.Domain.Entities;

public class Tour
{
    public Guid Id { get; set; }
    
    // Localized Fields
    public Dictionary<string, string> Names { get; set; } = new();
    public Dictionary<string, string> Descriptions { get; set; } = new();
    
    public decimal Price { get; set; }
    public string Duration { get; set; } = string.Empty;
    public List<string> Includes { get; set; } = new List<string>();
    public string ImageUrl { get; set; } = string.Empty;
    public string Emoji { get; set; } = string.Empty;
    public string BgGradient { get; set; } = string.Empty;
    public string Badge { get; set; } = string.Empty;
    
    public Guid DestinationId { get; set; }
    public Destination? Destination { get; set; }

    public Guid CategoryId { get; set; }
    public Category? Category { get; set; }
}
