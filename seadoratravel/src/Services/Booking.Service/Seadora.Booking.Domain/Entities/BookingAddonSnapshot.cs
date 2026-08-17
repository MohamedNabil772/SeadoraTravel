using System;

namespace Seadora.Booking.Domain.Entities;

public class BookingAddonSnapshot
{
    public Guid AddonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal TotalPrice => UnitPrice * Quantity;
}
