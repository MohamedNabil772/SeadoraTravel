namespace Seadora.Booking.Domain.Entities;

public class Booking
{
    public Guid Id { get; set; }
    public Guid TourId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; }
    public string Status { get; set; } = "Pending";
}
