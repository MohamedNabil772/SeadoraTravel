using System;

namespace Seadora.Booking.Domain.Entities;

public class GuestDetail
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? PassportFileName { get; set; }
    public string? AgeCategory { get; set; }
    public string? Nationality { get; set; }
    public string? SpecialRequests { get; set; }
}
