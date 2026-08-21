namespace Seadora.Content.Domain.Entities;

public class Language
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string NativeName { get; set; } = string.Empty;
    public string? FlagEmoji { get; set; }
    public bool IsRtl { get; set; }
    public bool IsDefault { get; set; }
    public int Order { get; set; }
    public bool IsActive { get; set; } = true;
}
