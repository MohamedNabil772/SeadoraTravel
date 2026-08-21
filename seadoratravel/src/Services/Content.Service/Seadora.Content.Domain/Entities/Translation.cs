using System;
using System.Collections.Generic;

namespace Seadora.Content.Domain.Entities;

public class Translation
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Namespace { get; set; } = string.Empty;
    public Dictionary<string, string> Values { get; set; } = new();
    public DateTime UpdatedAt { get; set; }
}
