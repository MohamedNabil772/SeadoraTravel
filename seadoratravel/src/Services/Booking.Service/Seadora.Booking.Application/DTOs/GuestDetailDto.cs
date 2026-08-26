using System;

namespace Seadora.Booking.Application.DTOs;

public class GuestDetailDto
{
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? PassportNumber { get; set; }
    public string? PassportFileName { get; set; }
    public string? AgeCategory { get; set; } = "Adult";
    public string? Nationality { get; set; }
    public string? SpecialRequests { get; set; }
    public bool HasIdentification => !string.IsNullOrWhiteSpace(PassportFileName) || !string.IsNullOrWhiteSpace(PassportNumber);
}
