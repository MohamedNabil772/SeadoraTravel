using System;

namespace Seadora.Booking.Application.DTOs;

public class FeedbackDto
{
    public Guid Id { get; set; }
    public Guid TourId { get; set; }
    public double Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsVisible { get; set; } = true;
}
