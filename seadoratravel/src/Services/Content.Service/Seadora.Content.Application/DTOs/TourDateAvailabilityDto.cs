using System;

namespace Seadora.Content.Application.DTOs;

public class TourDateAvailabilityDto
{
    public DateTime Date { get; set; }
    public bool IsAvailable { get; set; }
    public string Status { get; set; } = "Available"; // "Available", "LowStock", "SoldOut"
    public int SpotsLeft { get; set; }
    public decimal Price { get; set; }
}
