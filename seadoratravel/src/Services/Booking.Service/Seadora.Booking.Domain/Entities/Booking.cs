using System;
using Seadora.Booking.Domain.Enums;
using Seadora.Booking.Domain.ValueObjects;

namespace Seadora.Booking.Domain.Entities;

public class Booking
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
    public DateTime BookingDate { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public bool IsPaid { get; set; } = false;
    public string Attendance { get; set; } = "Pending";
    
    // Direct bookings & guest configuration
    public DateTime? TourDate { get; set; }
    public string? PickupTime { get; set; }
    public int Guests { get; set; } = 1;
    public bool HotelPickup { get; set; }
    public Guid? PackageId { get; set; }
    public decimal TotalPrice { get; set; }
    public string Language { get; set; } = "en";
    public bool MissingIdentification { get; set; }
    public List<BookingAddonSnapshot> SelectedAddons { get; set; } = new();
    public List<GuestDetail> GuestsList { get; set; } = new();

    // Snapshot of the branch / tour-type the booking was made under, for multi-branch reporting.
    public Guid BranchId { get; set; }
    public Guid? CustomerId { get; set; }
    public string? TourTypeCode { get; set; }
    // ponytail: nullable so pre-existing rows and any path that doesn't set it stay valid.
    public Money? Money { get; set; }
}
