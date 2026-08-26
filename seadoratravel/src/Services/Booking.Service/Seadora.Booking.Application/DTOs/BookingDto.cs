using System;
using Seadora.Booking.Domain.Enums;

namespace Seadora.Booking.Application.DTOs;

public class BookingDto
{
    public Guid Id { get; set; }
    public Guid TourId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string? WhatsApp { get; set; }
    public string? HotelName { get; set; }
    public string? RoomNumber { get; set; }
    public string? PassportFileName { get; set; }
    public string? TripType { get; set; }
    public string? PickupTime { get; set; }
    public DateTime? TourDate { get; set; }
    public int Guests { get; set; } = 1;
    public decimal TotalPrice { get; set; }
    public DateTime BookingDate { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public bool IsPaid { get; set; } = false;
    public string Attendance { get; set; } = "Pending";
    public bool HotelPickup { get; set; }
    public Guid? PackageId { get; set; }
    public bool MissingIdentification { get; set; }
    public List<Seadora.Booking.Domain.Entities.BookingAddonSnapshot> SelectedAddons { get; set; } = new();
    public List<GuestDetailDto> GuestsList { get; set; } = new();
}
